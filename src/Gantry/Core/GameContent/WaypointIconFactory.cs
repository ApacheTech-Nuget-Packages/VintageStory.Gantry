namespace Gantry.Core.GameContent;

/// <summary>
///     Provides functionality for creating and storing waypoint icons for the game.
/// </summary>
public static class WaypointIconFactory
{
    private static readonly Dictionary<string, LoadedTexture> _store = [];

    /// <summary>
    ///     Creates or retrieves a waypoint icon texture associated with the specified key.
    /// </summary>
    /// <param name="key">The unique key identifying the waypoint icon.</param>
    /// <returns>The <see cref="LoadedTexture"/> associated with the specified key.</returns>
    public static LoadedTexture Create(string key)
    {
        if (_store.TryGetValue(key, out var value)) return value;
        value = IOC.Services.GetRequiredService<WaypointMapLayer>().WaypointIcons[key]();
        _store.Add(key, value);
        return value;
    }

    /// <summary>
    ///     Attempts to create or retrieve a waypoint icon texture associated with the specified key.
    /// </summary>
    /// <param name="key">The unique key identifying the waypoint icon.</param>
    /// <param name="loadedTexture">The resulting <see cref="LoadedTexture"/> if the key is found, or <c>null</c> if not.</param>
    /// <returns><c>true</c> if the icon was successfully created or retrieved; otherwise, <c>false</c>.</returns>
    public static bool TryCreate(string key, out LoadedTexture? loadedTexture)
    {
        try
        {
            if (_store.TryGetValue(key, out loadedTexture)) return true;
            var waypointMapLayer = IOC.Services.GetRequiredService<WaypointMapLayer>();
            if (waypointMapLayer.WaypointIcons.TryGetValue(key, out var factory))
            {
                loadedTexture = factory();
                _store.Add(key, loadedTexture);
                return true;
            }
        }
        catch
        {
            G.Logger.VerboseDebug("Could not find a valid icon texture factor for '{0}'.", key);
        }

        loadedTexture = null;
        return false;
    }

    /// <summary>
    ///     Caches all waypoint icons, so that they don't stutter on world load.
    /// </summary>
    public static void PreCacheAllIcons(ICoreClientAPI capi)
    {
        var mapManager = capi.ModLoader.GetModSystem<WorldMapManager>();
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
    public static void Dispose()
    {
        _store.PurgeValues();
    }
}