using Gantry.Core.Abstractions;
using Gantry.GameContent.Extensions;

namespace Gantry.GameContent;

/// <summary>
///     Provides functionality for creating and storing waypoint icons for the game.
/// </summary>
public class WaypointIconService : IDisposable
{
    /// <summary>
    ///     Initialises a new instance of the <see cref="WaypointIconService"/> class.
    /// </summary>
    public WaypointIconService(ICoreGantryAPI coreApi)
    {
        _coreApi = coreApi;
    }

    private static readonly Dictionary<string, LoadedTexture> _store = [];
    private readonly ICoreGantryAPI _coreApi;

    /// <summary>
    ///     Creates or retrieves a waypoint icon texture associated with the specified key.
    /// </summary>
    /// <param name="key">The unique key identifying the waypoint icon.</param>
    /// <returns>The <see cref="LoadedTexture"/> associated with the specified key.</returns>
    public LoadedTexture Create(string key)
    {
        if (_store.TryGetValue(key, out var value)) return value;
        value = _coreApi.Services.GetRequiredService<WaypointMapLayer>().WaypointIcons[key]();
        _store.Add(key, value);
        return value;
    }

    /// <summary>
    ///     Attempts to create or retrieve a waypoint icon texture associated with the specified key.
    /// </summary>
    /// <param name="key">The unique key identifying the waypoint icon.</param>
    /// <param name="loadedTexture">The resulting <see cref="LoadedTexture"/> if the key is found, or <c>null</c> if not.</param>
    /// <returns><c>true</c> if the icon was successfully created or retrieved; otherwise, <c>false</c>.</returns>
    public bool TryCreate(string key, out LoadedTexture? loadedTexture)
    {
        try
        {
            if (_store.TryGetValue(key, out loadedTexture)) return true;
            var waypointMapLayer = _coreApi.Services.GetRequiredService<WaypointMapLayer>();
            if (waypointMapLayer.WaypointIcons.TryGetValue(key, out var factory))
            {
                loadedTexture = factory();
                _store.Add(key, loadedTexture);
                return true;
            }
        }
        catch
        {
            _coreApi.Logger.VerboseDebug("Could not find a valid icon texture factor for '{0}'.", key);
        }

        loadedTexture = null;
        return false;
    }

    /// <summary>
    ///     Caches all waypoint icons, so that they don't stutter on world load.
    /// </summary>
    public void PreCacheAllIcons()
    {
        var mapManager = _coreApi.Uapi.ModLoader.GetModSystem<WorldMapManager>();
        var waypointMapLayer = mapManager.WaypointMapLayer();
        if (waypointMapLayer is null) return;
        foreach (var (iconName, factory) in waypointMapLayer.WaypointIcons)
        {
            _store.Add(iconName, factory());
        }
    }

    /// <summary>
    ///     Disposes all textures, and clears the cache.
    /// </summary>
    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _store.PurgeValues();
    }
}