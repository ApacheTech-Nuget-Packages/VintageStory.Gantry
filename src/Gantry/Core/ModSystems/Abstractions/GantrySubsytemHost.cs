using ApacheTech.Common.DependencyInjection.Abstractions;
using ApacheTech.Common.DependencyInjection.Abstractions.Extensions;
using Gantry.Core.Hosting.Registration;
using Gantry.Core.ModSystems.Extensions;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

// ReSharper disable SuspiciousTypeConversion.Global

namespace Gantry.Core.ModSystems.Abstractions;

/// <summary>
///     Controls the loading of all <see cref="GantrySubsystem"/> instance within a mod.
/// </summary>
public abstract class GantrySubsytemHost : UniversalModSystem, IClientServiceRegistrar, IServerServiceRegistrar
{
    private IEnumerable<GantrySubsystem> _clientSubsystems;
    private IEnumerable<GantrySubsystem> _serverSubsystems;

    /// <inheritdoc />
    protected override void StartPreUniversal(ICoreAPI api)
    {
        ApiEx.OneOf(_clientSubsystems, _serverSubsystems).InvokeForAll(p => p.StartPre(api));
        base.StartPreUniversal(api);
    }

    /// <inheritdoc />
    public override void Start(ICoreAPI api)
    {
        ApiEx.OneOf(_clientSubsystems, _serverSubsystems).InvokeForAll(p => p.Start(api));
        base.Start(api);
    }

    /// <inheritdoc />
    public override void StartClientSide(ICoreClientAPI api)
    {
        _clientSubsystems.InvokeForAll(p => p.StartClientSide(api));
        base.StartClientSide(api);
    }

    /// <inheritdoc />
    public override void StartServerSide(ICoreServerAPI api)
    {
        _serverSubsystems.InvokeForAll(p => p.StartServerSide(api));
        base.StartServerSide(api);
    }

    /// <inheritdoc />
    public override void AssetsLoaded(ICoreAPI api)
    {
        ApiEx.OneOf(_clientSubsystems, _serverSubsystems).InvokeForAll(p => p.AssetsLoaded(api));
        base.AssetsLoaded(api);
    }

    /// <inheritdoc />
    public override void AssetsFinalize(ICoreAPI api)
    {
        ApiEx.OneOf(_clientSubsystems, _serverSubsystems).InvokeForAll(p => p.AssetsFinalize(api));
        base.AssetsFinalize(api);
    }

    /// <inheritdoc />
    void IServerServiceRegistrar.ConfigureServerModServices(IServiceCollection services, ICoreServerAPI sapi)
    {
        _serverSubsystems = ModEx.ModAssemblies.LoadGantrySubsystems(EnumAppSide.Server);
        _serverSubsystems.InvokeForAll(p => services.TryAddSingleton(p));
    }

    /// <inheritdoc />
    void IClientServiceRegistrar.ConfigureClientModServices(IServiceCollection services, ICoreClientAPI capi)
    {
        _clientSubsystems = ModEx.ModAssemblies.LoadGantrySubsystems(EnumAppSide.Client);
        _clientSubsystems.InvokeForAll(p => services.TryAddSingleton(p));
    }
}