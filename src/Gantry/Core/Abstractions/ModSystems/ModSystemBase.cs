using ApacheTech.Common.Extensions.Harmony;
using Gantry.Core.Helpers;
using Gantry.Extensions.Api;

namespace Gantry.Core.Abstractions.ModSystems;

/// <summary>
///     Base representation of a ModSystem used to extend Vintage Story.
/// </summary>
/// <seealso cref="ModSystem" />
public abstract class ModSystemBase<TModSystem> : ModSystem, IModSystem, IDisposable
    where TModSystem : ModSystemBase<TModSystem>
{
    private static Sided<TModSystem>? _instance;

    /// <summary>
    ///     Provides access to the current instance of the mod system.
    /// </summary>
    public static TModSystem? Instance => _instance?.Current;

    /// <inheritdoc />
    public ICoreAPI UApi { get; private set; } = null!;

    /// <inheritdoc />
    public override bool ShouldLoad(ICoreAPI api)
    {
        var modSsytem = GetType().Name;
        var shouldLoad = base.ShouldLoad(api);
        _instance ??= Sided<TModSystem>.AsyncLocal();
        if (_instance.Current is not null) return shouldLoad;
        _instance.Set(api.Side, this.To<TModSystem>());     
        OnShouldLoad(UApi = api);
        var host = Mod.Systems.Single(p => p is IModHost).To<IModHost>();
        Core = host.Gantry ?? throw new InvalidOperationException("The Gantry Core API is not available. Ensure that the mod is correctly set up to use Gantry.");
        return shouldLoad;
    }

    /// <summary>
    ///     This method is called when the mod system is loaded. It can be overridden in derived classes to perform additional actions.
    ///     The method is called after the mod system has been registered and before any other methods are called.
    /// </summary>
    protected virtual void OnShouldLoad(ICoreAPI api)
    {
        // INTENTIONALLY BLANK
    }

    /// <summary>
    ///     The Gantry Core API for the current mod and app side.
    /// </summary>
    protected ICoreGantryAPI Core { get; private set; } = null!;

    /// <inheritdoc />
    public override void StartPre(ICoreAPI api)
    {
        StartPreUniversal(api);
        switch (api)
        {
            case ICoreClientAPI capi:
                StartPreClientSide(capi);
                break;
            case ICoreServerAPI sapi:
                StartPreServerSide(sapi);
                break;
        }
    }

    /// <inheritdoc />
    public override double ExecuteOrder() => 0.05;

    /// <summary>
    ///     Called during initial mod loading, called before any mod receives the call to Start().
    /// </summary>
    protected virtual void StartPreUniversal(ICoreAPI api) { }

    /// <summary>
    ///     Called during initial mod loading, called before any mod receives the call to Start().
    /// </summary>
    protected virtual void StartPreServerSide(ICoreServerAPI sapi) { }

    /// <summary>
    ///     Called during initial mod loading, called before any mod receives the call to Start().
    /// </summary>
    protected virtual void StartPreClientSide(ICoreClientAPI capi) { }

    /// <inheritdoc />
    public override void Dispose()
    {
        GC.SuppressFinalize(this);
        _instance?.Dispose(UApi.Side);
        _instance = null!;
    }
}