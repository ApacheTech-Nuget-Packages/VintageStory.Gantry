using Gantry.Core.Extensions.DotNet;
using Gantry.Core.Hosting.Registration;
using Gantry.Core.ModSystems.Extensions;
using Vintagestory.API.Server;

// ReSharper disable SuspiciousTypeConversion.Global

namespace Gantry.Core.ModSystems.Abstractions;

/// <summary>
///     Controls the loading of all <see cref="GantrySubsystem"/> instance within a mod.
/// </summary>
public abstract class GantrySubsytemHost : UniversalModSystem
{
    private IEnumerable<GantrySubsystem> _subsystems;

    /// <inheritdoc />
    protected override void StartPreUniversal(ICoreAPI api)
    {
        Invoke(p => p.StartPre(api));
        base.StartPreUniversal(api);
    }

    /// <inheritdoc />
    protected override void StartPreClientSide(ICoreClientAPI capi)
    {
        Invoke(p => p.StartPreClientSide(capi));
        base.StartPreClientSide(capi);
    }

    /// <inheritdoc />
    protected override void StartPreServerSide(ICoreServerAPI sapi)
    {
        Invoke(p => p.StartPreServerSide(sapi));
        base.StartPreServerSide(sapi);
    }

    /// <inheritdoc />
    public override void Start(ICoreAPI api)
    {
        Invoke(p => p.Start(api));
        base.Start(api);
    }

    /// <inheritdoc />
    public override void StartClientSide(ICoreClientAPI api)
    {
        Invoke(p => p.StartClientSide(api));
        base.StartClientSide(api);
    }

    /// <inheritdoc />
    public override void StartServerSide(ICoreServerAPI api)
    {
        Invoke(p => p.StartServerSide(api));
        base.StartServerSide(api);
    }

    /// <inheritdoc />
    public override void AssetsLoaded(ICoreAPI api)
    {
        Invoke(p => p.AssetsLoaded(api));
        base.AssetsLoaded(api);
    }

    /// <inheritdoc />
    public override void AssetsFinalize(ICoreAPI api)
    {
        Invoke(p => p.AssetsFinalize(api));
        base.AssetsFinalize(api);
    }

    /// <inheritdoc cref="IUniversalServiceRegistrar.ConfigureUniversalModServices(IServiceCollection, ICoreAPI)" />
    protected virtual void ConfigureUniversalModServices(IServiceCollection services, ICoreAPI api)
    {
        var subsystems = ModEx.ModAssemblies.LoadGantrySubsystems();
        subsystems.For(api.Side).InvokeForAll(instance =>
        {
            instance.ConfigureUniversalModServices(services, api);
            ApiEx.Run(
                capi => instance.ConfigureClientModServices(services, capi),
                sapi => instance.ConfigureServerModServices(services, sapi));
            var serviceType = instance.GetType();
            var descriptor = ServiceDescriptor.Singleton(serviceType, instance);
            services.AddSingleton(instance);
            services.TryAdd(descriptor);
        });
    }

    /// <inheritdoc cref="IServerServiceRegistrar.ConfigureServerModServices(IServiceCollection, ICoreServerAPI)" />
    protected virtual void ConfigureServerModServices(IServiceCollection services, ICoreServerAPI sapi)
    {
    }

    /// <inheritdoc cref="IClientServiceRegistrar.ConfigureClientModServices(IServiceCollection, ICoreClientAPI)" />
    protected virtual void ConfigureClientModServices(IServiceCollection services, ICoreClientAPI capi)
    {
    }

    private void Invoke(Action<GantrySubsystem> action)
    {
        _subsystems ??= IOC.Services.GetServices<GantrySubsystem>();
        _subsystems.InvokeForAll(action);
    }
}