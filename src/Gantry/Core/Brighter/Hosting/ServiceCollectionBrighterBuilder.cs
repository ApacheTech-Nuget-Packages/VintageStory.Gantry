﻿using System.Reflection;
using ApacheTech.Common.BrighterSlim;
using ApacheTech.Common.DependencyInjection.Abstractions;
using Gantry.Core.Annotation;
using JetBrains.Annotations;
using Polly.Registry;
using Vintagestory.API.Common;

namespace Gantry.Core.Brighter.Hosting;

/// <summary>
///     Constructs Brighter message mappers and handlers
/// </summary>
/// <seealso cref="IBrighterBuilder" />
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class ServiceCollectionBrighterBuilder : IBrighterBuilder
{
    private readonly ServiceCollectionSubscriberRegistry _serviceCollectionSubscriberRegistry;
    private readonly ServiceCollectionMessageMapperRegistry _mapperRegistry;
    private readonly ServiceCollectionTransformerRegistry _transformerRegistry;

    /// <summary>
    ///     The policy registry to use for the command processor and the event bus
    ///     It needs to be here as we need to pass it between AddBrighter and UseExternalBus.
    /// </summary>
    /// <value>The policy registry.</value>
    public IPolicyRegistry<string> PolicyRegistry { get; set; }

    /// <summary>
    /// Registers the components of Brighter pipelines
    /// </summary>
    /// <param name="services">The IoC container to update</param>
    /// <param name="serviceCollectionSubscriberRegistry">The register for looking up message handlers</param>
    /// <param name="mapperRegistry">The register for looking up message mappers</param>
    /// <param name="transformerRegistry">The register for transforms</param>
    /// <param name="policyRegistry">The list of policies that we require</param>
    public ServiceCollectionBrighterBuilder(
        IServiceCollection services,
        ServiceCollectionSubscriberRegistry serviceCollectionSubscriberRegistry,
        ServiceCollectionMessageMapperRegistry mapperRegistry,
        ServiceCollectionTransformerRegistry transformerRegistry = null,
        IPolicyRegistry<string> policyRegistry = null
    )
    {
        Services = services;
        _serviceCollectionSubscriberRegistry = serviceCollectionSubscriberRegistry;
        _mapperRegistry = mapperRegistry;
        _transformerRegistry = transformerRegistry ?? new ServiceCollectionTransformerRegistry(services);
        PolicyRegistry = policyRegistry;
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
    public IBrighterBuilder AutoFromAssemblies(ICoreAPI api)
    {
        var assemblies = new[] { GetType().Assembly, ModEx.ModAssembly }.Distinct().ToArray();

        var side = api.Side;

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
            where ti.GetCustomAttribute<RunsOnAttribute>()?.ShouldRun(side) == true
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

        var subscribers =
            from ti in assemblies.SelectMany(a => a.DefinedTypes).Distinct()
            where ti.IsClass && !ti.IsAbstract && !ti.IsInterface
            from i in ti.ImplementedInterfaces
            where i.IsGenericType && i.GetGenericTypeDefinition() == interfaceType
            where ti.GetCustomAttribute<RunsOnAttribute>()?.ShouldRun(side) == true
            select new { RequestType = i.GenericTypeArguments.First(), HandlerType = ti.AsType() };

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
            where ti.GetCustomAttribute<RunsOnAttribute>()?.ShouldRun(side) == true
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
            where ti.GetCustomAttribute<RunsOnAttribute>()?.ShouldRun(side) == true
            select new { RequestType = i.GenericTypeArguments.First(), HandlerType = ti.AsType() };

        foreach (var mapper in mappers)
        {
            _mapperRegistry.AddAsync(mapper.RequestType, mapper.HandlerType);
        }
    }

}