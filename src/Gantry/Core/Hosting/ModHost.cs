using Gantry.Core.Abstractions;
using Gantry.Core.Abstractions.ModSystems;
using Vintagestory.Common;

namespace Gantry.Core.Hosting;

/// <summary>
///     Only one derived instance of this base-class should be added to any single mod within
///     the VintageMods domain. This base-class will enable Dependency Injection, and add all
///     the domain services. Derived instances should only have minimal functionality, 
///     instantiating, and adding Application specific services to the IOC Container.
/// </summary>
/// <seealso cref="ModSystem" />
public abstract class ModHost<TModSystem>(Action<GantryHostOptions>? options = null)
    : UniversalModSystem<TModSystem>, IModHost where TModSystem : ModHost<TModSystem>
{
    private readonly Action<GantryHostOptions>? _options = options;
    private readonly AsyncLocal<ICoreGantryAPI> _modCore = new();
    private bool _disposed;

    ICoreGantryAPI IModHost.Gantry => _modCore.Value!;

    /// <inheritdoc />
    protected override void OnShouldLoad(ICoreAPI api)
    {
        var core = GantryCore<TModSystem>.Create(api, (ModContainer)Mod);
        _modCore.Value = core;
        core.BuildServiceProvider(GantryHostOptions.Default.With(_options));

        _modCore.Value.Log(Nexus.TryAddCore(core)
            ? $"Gantry core for mod '{core.Mod.Info.ModID}' has been successfully registered within Gantry Nexus."
            : $"Gantry core for mod '{core.Mod.Info.ModID}' was not able to be registered within Gantry Nexus.");

        OnCoreLoaded(core);
    }

    /// <summary>
    ///     Fired once the Gantry core has been created, and the service provider built.
    /// </summary>
    /// <param name="core">The Gantry core API for the current mod and app side.</param>
    protected abstract void OnCoreLoaded(ICoreGantryAPI core);

    /// <summary>
    ///     Fired once the Gantry core has been unloaded, and the mod host is being disposed.
    /// </summary>
    protected abstract void OnCoreUnloaded();

    /// <summary>
    ///     Ensures that the mod host is the first mod system to be executed.
    /// </summary>
    public override double ExecuteOrder() => double.NegativeInfinity;

    /// <inheritdoc />
    public override void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _modCore.Value?.Log(Nexus.TryRemoveCore(_modCore.Value)
            ? $"Gantry core for mod '{_modCore.Value.Mod.Info.ModID}' has been successfully unregistered from Gantry Nexus."
            : $"Gantry core for mod '{_modCore.Value.Mod.Info.ModID}' was not able to be unregistered from Gantry Nexus.");

        _modCore.Value?.Log($"Disposing Gantry core for mod '{_modCore.Value.Mod.Info.ModID}'...");
        _modCore.Value?.Services.To<IDisposable>().Dispose();
        OnCoreUnloaded();
        base.Dispose();
        GetType().Assembly.NullifyOrphanedStaticMembers();

        GC.Collect();
        GC.WaitForPendingFinalizers();
    }
}