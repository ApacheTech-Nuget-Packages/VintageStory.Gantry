using System.Reflection;
using ApacheTech.Common.BrighterSlim;
using Polly.Registry;

namespace Gantry.Services.Brighter.Hosting;

/// <summary>
///     Constructs Brighter message mappers and handlers
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
internal interface IBrighterBuilder
{
    /// <summary>
    ///     Scan the assemblies provided for implementations of IHandleRequests, IHandleRequestsAsync, IAmAMessageMapper and register them with ServiceCollection.
    /// </summary>
    IBrighterBuilder AutoFromAssemblies(ICoreAPI api);

    /// <summary>
    ///     Scan the assemblies provided for implementations of IHandleRequestsAsync and register them with ServiceCollection.
    /// </summary>
    /// <param name="registerHandlers">A callback to register handlers</param>
    /// <returns>This builder, allows chaining calls</returns>
    IBrighterBuilder AsyncHandlers(Action<IAmAnAsyncSubcriberRegistry> registerHandlers);

    /// <summary>
    ///     Scan the assemblies provided for implementations of IHandleRequests and register them with ServiceCollection.
    /// </summary>
    /// <param name="side">Determines the app side to register the handlers on.</param>
    /// <param name="assemblies">The assemblies to scan</param>
    /// <returns>This builder, allows chaining calls</returns>
    IBrighterBuilder AsyncHandlersFromAssemblies(EnumAppSide side, params Assembly[] assemblies);

    /// <summary>
    ///     Register handlers with the built-in subscriber registry
    /// </summary>
    /// <param name="registerHandlers">A callback to register handlers</param>
    /// <returns>This builder, allows chaining calls</returns>
    IBrighterBuilder Handlers(Action<IAmASubscriberRegistry> registerHandlers);

    /// <summary>
    ///     Scan the assemblies provided for implementations of IHandleRequests and register them with ServiceCollection.
    /// </summary>
    /// <param name="side">Determines the app side to register the handlers on.</param>
    /// <param name="assemblies">The assemblies to scan</param>
    /// <returns>This builder, allows chaining calls</returns>
    IBrighterBuilder HandlersFromAssemblies(EnumAppSide side, params Assembly[] assemblies);

    /// <summary>
    ///     Register message mappers.
    /// </summary>
    /// <param name="registerMappers">A callback to register mappers</param>
    /// <returns>This builder, allows chaining calls</returns>
    IBrighterBuilder MapperRegistry(Action<ServiceCollectionMessageMapperRegistry> registerMappers);

    /// <summary>
    ///     Scan the assemblies provided for implementations of IAmAMessageMapper and register them with ServiceCollection.
    /// </summary>
    /// <param name="side">Determines the app side to register the mappers on.</param>
    /// <param name="assemblies">The assemblies to scan</param>
    /// <returns>This builder, allows chaining calls</returns>
    IBrighterBuilder MapperRegistryFromAssemblies(EnumAppSide side, params Assembly[] assemblies);

    /// <summary>
    ///     Scan the assemblies for implementations of IAmAMessageTransformAsync and register them with ServiceCollection.
    /// </summary>
    /// <param name="side">Determines the app side to register the transforms on.</param>
    /// <param name="assemblies">The assemblies to scan</param>
    /// <returns>This builder, allows chaining calls</returns>
    IBrighterBuilder TransformsFromAssemblies(EnumAppSide side, params Assembly[] assemblies);

    /// <summary>
    ///     The policy registry to use for the command processor and the event bus
    ///     It needs to be here as we need to pass it between AddBrighter and UseExternalBus.
    /// </summary>
    IPolicyRegistry<string> PolicyRegistry { get; set; }


    /// <summary>
    ///     The IoC container to populate.
    /// </summary>
    IServiceCollection Services { get; }
}