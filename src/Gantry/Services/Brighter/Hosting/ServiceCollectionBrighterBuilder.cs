using ApacheTech.Common.BrighterSlim;
using Gantry.Core.Abstractions;
using Gantry.Core.Annotation;

namespace Gantry.Services.Brighter.Hosting;

/// <summary>
///     Constructs Brighter message mappers and handlers
/// </summary>
/// <seealso cref="IBrighterBuilder" />
internal class ServiceCollectionBrighterBuilder : IBrighterBuilder
{
    private readonly ServiceCollectionSubscriberRegistry _serviceCollectionSubscriberRegistry;
    private readonly ServiceCollectionMessageMapperRegistry _mapperRegistry;
    private readonly ServiceCollectionTransformerRegistry _transformerRegistry;

    /// <summary>
    /// Registers the components of Brighter pipelines
    /// </summary>
    /// <param name="services">The IoC container to update</param>
    /// <param name="serviceCollectionSubscriberRegistry">The register for looking up message handlers</param>
    /// <param name="mapperRegistry">The register for looking up message mappers</param>
    /// <param name="transformerRegistry">The register for transforms</param>
    public ServiceCollectionBrighterBuilder(
        IServiceCollection services,
        ServiceCollectionSubscriberRegistry serviceCollectionSubscriberRegistry,
        ServiceCollectionMessageMapperRegistry mapperRegistry,
        ServiceCollectionTransformerRegistry? transformerRegistry = null
    )
    {
        Services = services;
        _serviceCollectionSubscriberRegistry = serviceCollectionSubscriberRegistry;
        _mapperRegistry = mapperRegistry;
        _transformerRegistry = transformerRegistry ?? new ServiceCollectionTransformerRegistry(services);
    }

    /// <summary>
    ///     The IoC container we are populating
    /// </summary>
    public IServiceCollection Services { get; }

    /// <summary>
    ///     Scan the assemblies provided for implementations of IHandleRequestsAsync and register them with ServiceCollection
    /// </summary>
    /// <param name="registerHandlers">A callback to register handlers</param>
    /// <returns>This builder, allows chaining calls</returns>
    public IBrighterBuilder AsyncHandlers(Action<IAmAnAsyncSubcriberRegistry> registerHandlers)
    {
        if (registerHandlers == null)
            throw new ArgumentNullException(nameof(registerHandlers));

        registerHandlers(_serviceCollectionSubscriberRegistry);

        return this;
    }

    /// <summary>
    ///     Scan the assemblies provided for implementations of IHandleRequests and register them with ServiceCollection 
    /// </summary>
    /// <param name="side">Determines the app side to register the handlers on.</param>
    /// <param name="assemblies">The assemblies to scan</param>
    /// <returns>This builder, allows chaining calls</returns>
    public IBrighterBuilder AsyncHandlersFromAssemblies(EnumAppSide side, params Assembly[] assemblies)
    {
        RegisterHandlersFromAssembly(side, typeof(IHandleRequestsAsync<>), assemblies, typeof(IHandleRequestsAsync<>).Assembly);
        return this;
    }

    /// <summary>
    ///     Scan the assemblies provided for implementations of IHandleRequests, IHandleRequestsAsync, IAmAMessageMapper and register them with ServiceCollection
    /// </summary>
    /// <returns></returns>
    public IBrighterBuilder AutoFromAssemblies(ICoreGantryAPI core)
    {
        var assemblies = core.ModAssemblies.ToArray();
        var side = core.Uapi.Side;

        MapperRegistryFromAssemblies(side, assemblies);
        HandlersFromAssemblies(side, assemblies);
        AsyncHandlersFromAssemblies(side, assemblies);
        TransformsFromAssemblies(side, assemblies);

        return this;
    }

    /// <summary>
    ///     Register message mappers
    /// </summary>
    /// <param name="registerMappers">A callback to register mappers</param>
    /// <returns></returns>
    public IBrighterBuilder MapperRegistry(Action<ServiceCollectionMessageMapperRegistry> registerMappers)
    {
        if (registerMappers == null) throw new ArgumentNullException(nameof(registerMappers));

        registerMappers(_mapperRegistry);

        return this;
    }

    /// <summary>
    ///     Scan the assemblies provided for implementations of IAmAMessageMapper and register them with ServiceCollection
    /// </summary>
    /// <param name="side">Determines the app side to register the mappers on.</param>
    /// <param name="assemblies">The assemblies to scan</param>
    /// <returns>This builder, allows chaining calls</returns>
    public IBrighterBuilder MapperRegistryFromAssemblies(EnumAppSide side, params Assembly[] assemblies)
    {
        if (assemblies.Length == 0)
            throw new ArgumentException("Value cannot be an empty collection.", nameof(assemblies));

        RegisterMappersFromAssemblies(side, assemblies);
        RegisterAsyncMappersFromAssemblies(side, assemblies);

        return this;
    }


    /// <summary>
    ///     Register handlers with the built-in subscriber registry
    /// </summary>
    /// <param name="registerHandlers">A callback to register handlers</param>
    /// <returns>This builder, allows chaining calls</returns>
    public IBrighterBuilder Handlers(Action<IAmASubscriberRegistry> registerHandlers)
    {
        if (registerHandlers == null)
            throw new ArgumentNullException(nameof(registerHandlers));

        registerHandlers(_serviceCollectionSubscriberRegistry);

        return this;
    }

    /// <summary>
    ///     Scan the assemblies provided for implementations of IHandleRequests and register them with ServiceCollection
    /// </summary>
    /// <param name="side">Determines the app side to register the handlers on.</param>
    /// <param name="assemblies">The assemblies to scan</param>
    /// <returns>This builder, allows chaining calls</returns>
    public IBrighterBuilder HandlersFromAssemblies(EnumAppSide side, params Assembly[] assemblies)
    {
        RegisterHandlersFromAssembly(side, typeof(IHandleRequests<>), assemblies, typeof(IHandleRequests<>).Assembly);
        return this;
    }

    /// <summary>
    ///     Scan the assemblies for implementations of IAmAMessageTransformAsync and register them with the ServiceCollection
    /// </summary>
    /// <param name="side">Determines the app side to register the transforms on.</param>
    /// <param name="assemblies">The assemblies to scan</param>
    /// <returns>This builder, allows chaining calls</returns>
    /// <exception cref="ArgumentException">Thrown if there are no assemblies passed to the method</exception>
    public IBrighterBuilder TransformsFromAssemblies(EnumAppSide side, params Assembly[] assemblies)
    {
        if (assemblies.Length == 0)
            throw new ArgumentException("Value cannot be an empty collection.", nameof(assemblies));

        var transforms =
            from ti in assemblies.SelectMany(a => a.DefinedTypes).Distinct()
            where ti.IsClass && !ti.IsAbstract && !ti.IsInterface
            from i in ti.ImplementedInterfaces
            where i == typeof(IAmAMessageTransformAsync)
            where ti.GetCustomAttribute<SidedAttribute>()?.For(side) == true
            select new { TransformType = ti.AsType() };

        foreach (var transform in transforms)
        {
            _transformerRegistry.Add(transform.TransformType);
        }

        return this;
    }

    private void RegisterHandlersFromAssembly(EnumAppSide side, Type interfaceType, IEnumerable<Assembly> assemblies, Assembly assembly)
    {
        assemblies = assemblies.Concat([assembly]).ToArray();

        // Step 1: Get all defined types from all assemblies and flatten them into a single collection.
        var definedTypes = assemblies.SelectMany(a => a.DefinedTypes).ToList();

        // Step 2: Remove duplicate types to ensure distinct defined types.
        var distinctTypes = definedTypes.Distinct().ToList();

        // Step 3: Filter the types to include only classes that are not abstract and not interfaces.
        var classTypes = distinctTypes.Where(ti => ti.IsClass && !ti.IsAbstract && !ti.IsInterface).ToList();

        // Step 4: Select the implemented interfaces of each class.
        var implementedInterfaces = classTypes.SelectMany(ti => ti.ImplementedInterfaces, (ti, i) => new { TypeInfo = ti, Interface = i }).ToList();

        // Step 5: Filter the implemented interfaces to include only those that are generic and match the specified generic interface type.
        var matchingInterfaces = implementedInterfaces.Where(x => x.Interface.IsGenericType && x.Interface.GetGenericTypeDefinition() == interfaceType).ToList();

        // Step 6: Further filter to only include types that have the `RunsOnAttribute` and where `ShouldRun(side)` returns true.
        var filteredTypes = matchingInterfaces.Where(x =>
        {
            var attribute = x.TypeInfo.GetCustomAttribute<SidedAttribute>();
            if (attribute is null) return true;
            var shouldRun = attribute.For(side);
            return shouldRun;
        }).ToList();

        // Step 7: Select the desired result, which includes the request type and the handler type.
        var subscribers = filteredTypes.Select(x => new { RequestType = x.Interface.GenericTypeArguments.First(), HandlerType = x.TypeInfo.AsType() }).ToList();

        foreach (var subscriber in subscribers)
        {
            _serviceCollectionSubscriberRegistry.Add(subscriber.RequestType, subscriber.HandlerType);
        }
    }

    private void RegisterMappersFromAssemblies(EnumAppSide side, Assembly[] assemblies)
    {
        var mappers =
            from ti in assemblies.SelectMany(a => a.DefinedTypes).Distinct()
            where ti.IsClass && !ti.IsAbstract && !ti.IsInterface
            from i in ti.ImplementedInterfaces
            where i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IAmAMessageMapper<>)
            where ti.GetCustomAttribute<SidedAttribute>()?.For(side) == true
            select new { RequestType = i.GenericTypeArguments.First(), HandlerType = ti.AsType() };

        foreach (var mapper in mappers)
        {
            _mapperRegistry.Add(mapper.RequestType, mapper.HandlerType);
        }
    }

    private void RegisterAsyncMappersFromAssemblies(EnumAppSide side, Assembly[] assemblies)
    {
        var mappers =
            from ti in assemblies.SelectMany(a => a.DefinedTypes).Distinct()
            where ti.IsClass && !ti.IsAbstract && !ti.IsInterface
            from i in ti.ImplementedInterfaces
            where i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IAmAMessageMapperAsync<>)
            where ti.GetCustomAttribute<SidedAttribute>()?.For(side) == true
            select new { RequestType = i.GenericTypeArguments.First(), HandlerType = ti.AsType() };

        foreach (var mapper in mappers)
        {
            _mapperRegistry.AddAsync(mapper.RequestType, mapper.HandlerType);
        }
    }

}