using Gantry.Core.Helpers;
using Gantry.Extensions.Api;

namespace Gantry.Core.Abstractions.ModSystems;

/// <summary>
///     Base representation of a ModSystem used to extend Vintage Story.
/// </summary>
/// <seealso cref="ModSystem" />
public abstract class ModSystemBase<TModSystem> : ModSystem, IHostedModSystem
    where TModSystem : ModSystemBase<TModSystem>
{
    private static readonly Sided<TModSystem> _instance = new();
    private bool _disposed;

    /// <summary>
    ///     Provides access to the current instance of the mod system.
    /// </summary>
    public static TModSystem Instance => _instance.Current;

    /// <summary>
    ///     Provides access to the current instance of the client-side mod system.
    /// </summary>
    public static TModSystem Client { get; private set; } = default!;

    /// <summary>
    ///     Provides access to the current instance of the server-side mod system.
    /// </summary>
    public static TModSystem Server { get; private set; } = default!;

    /// <inheritdoc />
    public ICoreAPI UApi { get; private set; } = null!;

    /// <summary>
    ///     The Gantry Core API for the current mod and app side.
    /// </summary>
    public ICoreGantryAPI Core { get; private set; } = null!;

    /// <summary>
    ///     Sets the Gantry Core API for the current mod and app side.
    /// </summary>
    /// <param name="core">The Gantry Core API for the current mod and app side.</param>
    void IHostedModSystem.SetCore(ICoreGantryAPI core)
    {
        var modSystemName = GetType().Name;
        Core = core;
    }

    /// <inheritdoc />
    public override void StartPre(ICoreAPI api)
    {
        UApi = api;
        var instance = (TModSystem)this;
        _instance.Set(api.Side, instance);
        api.Side.Invoke(
            () => Client = instance, 
            () => Server = instance);

        if (Instance is IModHost host)
            host.InitialiseCore(api);

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
        if (_disposed) return;
        _disposed = true;
        if (UApi is null || Instance != this) return;
        _instance.Dispose(UApi.Side);
    }
}