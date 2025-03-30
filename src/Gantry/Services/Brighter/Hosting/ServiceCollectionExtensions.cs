using System.Text.Json;
using ApacheTech.Common.BrighterSlim;
using ApacheTech.Common.BrighterSlim.FeatureSwitch;
using Gantry.Core.Hosting.Extensions;

// ReSharper disable StringLiteralTypo

namespace Gantry.Services.Brighter.Hosting;

/// <summary>
///     Service Collection Extensions.
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
internal static class ServiceCollectionExtensions
{
    /// <summary>
    /// Will add Brighter into the .NET IoC Container - ServiceCollection
    /// Registers the following with the service collection :-
    ///  - BrighterOptions - how should we configure Brighter
    ///  - Feature Switch Registry - optional if features switch support is desired
    ///  - Inbox - defaults to InMemoryInbox if none supplied 
    ///  - SubscriberRegistry - what handlers subscribe to what requests
    ///  - MapperRegistry - what mappers translate what messages
    /// </summary>
    /// <param name="services">The collection of services that we want to add registrations to</param>
    /// <param name="configure">A callback that defines what options to set when Brighter is built</param>
    /// <returns>A builder that can be used to populate the IoC container with handlers and mappers by inspection
    /// - used by built in factory from CommandProcessor</returns>
    /// <exception cref="ArgumentNullException">Thrown if we have no IoC provided ServiceCollection</exception>
    public static IBrighterBuilder AddBrighter(
        this IServiceCollection services,
        Action<BrighterOptions>? configure = null)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        var options = new BrighterOptions();
        configure?.Invoke(options);
        services.TryAddSingleton<IBrighterOptions>(options);

        return BrighterHandlerBuilder(services, options);
    }

    /// <summary>
    /// Registers the following with the service collection :-
    ///  - BrighterOptions - how should we configure Brighter
    ///  - Feature Switch Registry - optional if features switch support is desired
    ///  - Inbox - defaults to InMemoryInbox if none supplied 
    ///  - SubscriberRegistry - what handlers subscribe to what requests
    ///  - MapperRegistry - what mappers translate what messages
    /// </summary>
    /// <param name="services">The collection of services that we want to add registrations to</param>
    /// <param name="options"></param>
    /// <returns></returns>
    private static IBrighterBuilder BrighterHandlerBuilder(IServiceCollection services, BrighterOptions options)
    {
        var subscriberRegistry = new ServiceCollectionSubscriberRegistry(services, options.HandlerLifetime);
        services.TryAddSingleton(subscriberRegistry);

        var transformRegistry = new ServiceCollectionTransformerRegistry(services, options.TransformerLifetime);
        services.TryAddSingleton(transformRegistry);

        var mapperRegistry = new ServiceCollectionMessageMapperRegistry(services, options.MapperLifetime);
        services.TryAddSingleton(mapperRegistry);

        if (options.FeatureSwitchRegistry != null)
            services.TryAddSingleton(options.FeatureSwitchRegistry);

        services.TryAdd(new ServiceDescriptor(typeof(IAmACommandProcessor),
            (serviceProvider) => (IAmACommandProcessor)BuildCommandProcessor(serviceProvider),
            options.CommandProcessorLifetime));

        return new ServiceCollectionBrighterBuilder(
            services,
            subscriberRegistry,
            mapperRegistry,
            transformRegistry
        );
    }

    /// <summary>
    /// An external bus is the use of Message Oriented Middleware (MoM) to dispatch a message between a producer
    /// and a consumer. The assumption is that this  is being used for inter-process communication, for example the
    /// work queue pattern for distributing work, or between micro-services
    /// Registers singletons with the service collection :-
    /// - An Event Bus - used to send message externally and contains:
    ///     -- Producer Registry - A list of producers we can send middleware messages with 
    ///     -- Outbox - stores messages so that they can be written in the same transaction as entity writes
    ///     -- Outbox Transaction Provider - used to provide a transaction that spans the Outbox write and
    ///         your updates to your entities
    ///     -- RelationalDb Connection Provider - if your transaction provider is for a relational db we register this
    ///         interface to access your Db and make it available to your own classes
    ///     -- Transaction Connection Provider  - if your transaction provider is also a relational db connection
    ///         provider it will implement this interface which inherits from both
    ///     -- External Bus Configuration - the configuration parameters for an external bus, mainly used internally
    ///     -- UseRpc - do we want to use RPC i.e. a command blocks waiting for a response, over middleware.
    /// </summary>
    /// <param name="brighterBuilder">The Brighter builder to add this option to</param>
    /// <param name="configure">A callback that allows you to configure <see cref="ExternalBusConfiguration"/> options</param>
    /// <param name="serviceLifetime">The lifetime of the transaction provider</param>
    /// <returns>The Brighter builder to allow chaining of requests</returns>
    public static IBrighterBuilder UseExternalBus(
        this IBrighterBuilder brighterBuilder,
        Action<ExternalBusConfiguration> configure,
        ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
    {
        if (brighterBuilder is null)
            throw new ArgumentNullException(nameof(brighterBuilder), $"{nameof(brighterBuilder)} cannot be null.");

        var busConfiguration = new ExternalBusConfiguration();
        configure?.Invoke(busConfiguration);
        brighterBuilder.Services.TryAddSingleton<IAmExternalBusConfiguration>(busConfiguration);

        //default to using System Transactions if nothing provided, so we always technically can share the outbox transaction
        var transactionProvider = busConfiguration.TransactionProvider ?? typeof(CommittableTransactionProvider);

        //Find the transaction type from the provider
        var transactionProviderInterface = typeof(IAmABoxTransactionProvider<>);
        Type? transactionType = null;
        foreach (var i in transactionProvider.GetInterfaces())
            if (i.IsGenericType && i.GetGenericTypeDefinition() == transactionProviderInterface)
                transactionType = i.GetGenericArguments()[0];

        if (transactionType is null)
            throw new ConfigurationException(
                $"Unable to register provider of type {transactionProvider.Name}. It does not implement {typeof(IAmABoxTransactionProvider<>).Name}.");

        //register the generic interface with the transaction type
        var boxProviderType = transactionProviderInterface.MakeGenericType(transactionType);

        brighterBuilder.Services.Add(new ServiceDescriptor(boxProviderType, transactionProvider, serviceLifetime));

        //NOTE: It is a little unsatisfactory to hard code our types in here
        RegisterRelationalProviderServicesMaybe(brighterBuilder, busConfiguration.ConnectionProvider, transactionProvider, serviceLifetime);

        return ExternalBusBuilder(brighterBuilder, busConfiguration, transactionType);
    }

    private static object BuildCommandProcessor(IServiceProvider provider)
    {
        var options = provider.Resolve<IBrighterOptions>();
        var subscriberRegistry = provider.GetService<ServiceCollectionSubscriberRegistry>();
        var useRequestResponse = provider.GetService<IUseRpc>();

        var handlerFactory = new ServiceProviderHandlerFactory(provider);
        var handlerConfiguration = new HandlerConfiguration(subscriberRegistry, handlerFactory);

        var needHandlers = CommandProcessorBuilder.With();

        provider.TryResolve<IAmAFeatureSwitchRegistry>(out var featureSwitchRegistry);

        if (featureSwitchRegistry is not null)
            needHandlers = needHandlers.ConfigureFeatureSwitches(featureSwitchRegistry);

        var policyBuilder = needHandlers.Handlers(handlerConfiguration);

        var ret = AddEventBus(provider, policyBuilder, useRequestResponse);

        var commandProcessor = ret
            .RequestContextFactory(options.RequestContextFactory)
            .Build();

        return commandProcessor;
    }

    private static INeedARequestContext AddEventBus(
        IServiceProvider provider,
        INeedMessaging messagingBuilder,
        IUseRpc? useRequestResponse)
    {
        provider.TryResolve<IAmExternalBusConfiguration>(out var eventBusConfiguration);
        provider.TryResolve<IServiceActivatorOptions>(out var serviceActivatorOptions);
        var messageMapperRegistry = MessageMapperRegistry(provider);
        var messageTransformFactory = TransformFactory(provider);
        var hasEventBus = provider.TryResolve<IAmAnExternalBusService>(out var eventBus);
        var useRpc = useRequestResponse is { RPC: true };

        var ret = hasEventBus switch
        {
            false => messagingBuilder.NoExternalBus(),
            true when !useRpc => messagingBuilder.ExternalBus(ExternalBusType.FireAndForget, eventBus,
                messageMapperRegistry, messageTransformFactory, eventBusConfiguration?.ResponseChannelFactory,
                eventBusConfiguration?.ReplyQueueSubscriptions, serviceActivatorOptions?.InboxConfiguration),
            _ => throw new UnreachableException()
        };

        if (hasEventBus && useRpc)
        {
            ret = messagingBuilder.ExternalBus(
                ExternalBusType.RPC,
                eventBus,
                messageMapperRegistry,
                messageTransformFactory,
                eventBusConfiguration?.ResponseChannelFactory,
                eventBusConfiguration?.ReplyQueueSubscriptions,
                serviceActivatorOptions?.InboxConfiguration
            );
        }

        return ret;
    }

    private static IBrighterBuilder ExternalBusBuilder(
        IBrighterBuilder brighterBuilder,
        IAmExternalBusConfiguration externalBusConfiguration,
        Type transactionType)
    {
        if (externalBusConfiguration.ProducerRegistry is null)
            throw new ConfigurationException("An external bus must have an IAmAProducerRegistry");

        var serviceCollection = brighterBuilder.Services;

        serviceCollection.TryAddSingleton(externalBusConfiguration);
        serviceCollection.TryAddSingleton(externalBusConfiguration.ProducerRegistry);

        //we always need an outbox in case of producer callbacks
        var outbox = externalBusConfiguration.Outbox ?? new InMemoryOutbox();

        //we create the outbox from interfaces from the determined transaction type to prevent the need
        //to pass generic types as we know the transaction provider type
        var syncOutboxType = typeof(IAmAnOutboxSync<,>).MakeGenericType(typeof(Message), transactionType);
        var asyncOutboxType = typeof(IAmAnOutboxAsync<,>).MakeGenericType(typeof(Message), transactionType);

        foreach (var i in outbox.GetType().GetInterfaces())
        {
            if (i.IsGenericType && i.GetGenericTypeDefinition() == syncOutboxType)
            {
                var outboxDescriptor = new ServiceDescriptor(syncOutboxType, _ => outbox, ServiceLifetime.Singleton);
                serviceCollection.Add(outboxDescriptor);
            }

            if (!i.IsGenericType || i.GetGenericTypeDefinition() != asyncOutboxType) continue;
            var asyncOutboxDescriptor = new ServiceDescriptor(asyncOutboxType, _ => outbox, ServiceLifetime.Singleton);
            serviceCollection.Add(asyncOutboxDescriptor);
        }

        if (externalBusConfiguration.UseRpc)
        {
            serviceCollection.TryAddSingleton<IUseRpc>(new UseRpc(externalBusConfiguration.UseRpc,
                externalBusConfiguration.ReplyQueueSubscriptions));
        }

        //Because the bus has specialized types as members, we need to create the bus type dynamically
        //again to prevent someone configuring Brighter from having to pass generic types
        var busType = typeof(ExternalBusServices<,>).MakeGenericType(typeof(Message), transactionType);

        var bus = Activator.CreateInstance(busType,
            externalBusConfiguration.ProducerRegistry,
            outbox,
            externalBusConfiguration.OutboxBulkChunkSize,
            externalBusConfiguration.OutboxTimeout) as IAmAnExternalBusService;

        serviceCollection.TryAddSingleton(bus);

        return brighterBuilder;
    }

    /// <summary>
    ///     Configure the JSON Serialiser that is used inside Brighter
    /// </summary>
    /// <param name="brighterBuilder">The Brighter Builder</param>
    /// <param name="configure">Action to configure the options</param>
    /// <returns>Brighter Builder</returns>
    public static IBrighterBuilder ConfigureJsonSerialisation(this IBrighterBuilder brighterBuilder,
        Action<JsonSerializerOptions> configure)
    {
        var options = new JsonSerializerOptions();

        configure.Invoke(options);

        JsonSerialisationOptions.Options = options;

        return brighterBuilder;
    }

    /// <summary>
    /// Registers message mappers with the registry. Normally you don't need to call this, it is called by the builder for Brighter or the Service Activator
    /// Visibility is required for use from both
    /// </summary>
    /// <param name="provider">The IoC container to request the message mapper registry from</param>
    /// <returns>The message mapper registry, populated with any message mappers from the ioC container</returns>
    public static MessageMapperRegistry MessageMapperRegistry(IServiceProvider provider)
    {
        var serviceCollectionMessageMapperRegistry = provider.Resolve<ServiceCollectionMessageMapperRegistry>();

        var messageMapperRegistry = new MessageMapperRegistry(
            new ServiceProviderMapperFactory(provider),
            new ServiceProviderMapperFactoryAsync(provider)
        );

        foreach (var messageMapper in serviceCollectionMessageMapperRegistry.Mappers)
        {
            messageMapperRegistry.Register(messageMapper.Key, messageMapper.Value);
        }

        foreach (var messageMapper in serviceCollectionMessageMapperRegistry.AsyncMappers)
        {
            messageMapperRegistry.RegisterAsync(messageMapper.Key, messageMapper.Value);
        }

        return messageMapperRegistry;
    }

    private static void RegisterRelationalProviderServicesMaybe(
        IBrighterBuilder brighterBuilder,
        Type connectionProvider,
        Type transactionProvider,
        ServiceLifetime serviceLifetime
    )
    {
        //not all box transaction providers are also relational connection providers
        if (typeof(IAmARelationalDbConnectionProvider).IsAssignableFrom(connectionProvider))
        {
            brighterBuilder.Services.Add(new ServiceDescriptor(typeof(IAmARelationalDbConnectionProvider),
                connectionProvider, serviceLifetime));
        }

        //not all box transaction providers are also relational connection providers
        if (typeof(IAmATransactionConnectionProvider).IsAssignableFrom(transactionProvider))
        {
            //register the combined interface just in case
            brighterBuilder.Services.Add(new ServiceDescriptor(typeof(IAmATransactionConnectionProvider),
                transactionProvider, serviceLifetime));
        }
    }

    /// <summary>
    /// Creates transforms. Normally you don't need to call this, it is called by the builder for Brighter or
    /// the Service Activator
    /// Visibility is required for use from both
    /// </summary>
    /// <param name="provider">The IoC container to build the transform factory over</param>
    /// <returns></returns>
    public static ServiceProviderTransformerFactory TransformFactory(IServiceProvider provider)
    {
        return new ServiceProviderTransformerFactory(provider);
    }

    /// <summary>
    /// Creates transforms. Normally you don't need to call this, it is called by the builder for Brighter or
    /// the Service Activator
    /// Visibility is required for use from both
    /// </summary>
    /// <param name="provider">The IoC container to build the transform factory over</param>
    /// <returns></returns>
    public static ServiceProviderTransformerFactoryAsync TransformFactoryAsync(IServiceProvider provider)
    {
        return new ServiceProviderTransformerFactoryAsync(provider);
    }
}