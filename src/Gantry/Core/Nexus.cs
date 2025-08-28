using Gantry.Core.Abstractions;
using Gantry.Extensions.Api;
using System.Collections.Concurrent;

namespace Gantry.Core;

/// <summary>
///     Provides inter-mod functionality for Gantrified mods.
/// </summary>
public static partial class Nexus
{
    private static readonly ConcurrentDictionary<string, ICoreGantryAPI> _client = [];
    private static readonly ConcurrentDictionary<string, ICoreGantryAPI> _server = [];

    /// <summary>
    ///     Adds the provided <see cref="ICoreGantryAPI"/> to the list of active mods.
    /// </summary>
    /// <param name="api">The core API instance to add.</param>
    /// <returns>True if the core was added; otherwise, false.</returns>
    public static bool TryAddCore(ICoreGantryAPI api)
    {
        var added = api.Side.Invoke(
                () => _client.TryAdd(api.Mod.Info.ModID, api),
                () => _server.TryAdd(api.Mod.Info.ModID, api));

        // ROADMAP: Can add cross-mod networking and eventing here later.

        return added;
    }

    /// <summary>
    ///     Removes the provided <see cref="ICoreGantryAPI"/> from the list of active mods.
    /// </summary>
    /// <param name="api">The core API instance to remove.</param>
    /// <returns>True if the core was removed; otherwise, false.</returns>
    public static bool TryRemoveCore(ICoreGantryAPI api)
    {
        var removed = api.Side.Invoke(
                () => _client.TryRemove(api.Mod.Info.ModID, out _),
                () => _server.TryRemove(api.Mod.Info.ModID, out _));

        // ROADMAP: Can add cross-mod networking and eventing here later.

        return removed;
    }

    /// <summary>
    ///     Determines if a mod with the specified mod ID is installed on the given side.
    /// </summary>
    /// <param name="modId">The mod ID to check for.</param>
    /// <param name="side">The side (client or server) to check.</param>
    /// <returns>Whether the mod is installed on the specified side.</returns>
    public static bool IsInstalled(string modId, EnumAppSide side) 
        => side.Invoke(
            () => _client.ContainsKey(modId), 
            () => _server.ContainsKey(modId));

    /// <summary>
    ///     Provides access to the <see cref="ICoreGantryAPI"/> for the specified mod ID and side.
    /// </summary>
    /// <param name="modId">The mod ID of the mod to retrieve the core for.</param>
    /// <param name="side">The side (client or server) for which to retrieve the core.</param>
    /// <param name="core">When this method returns, contains the <see cref="ICoreGantryAPI"/> instance if found; otherwise, null.</param>
    /// <returns>True if the core was found; otherwise, false.</returns>
    public static bool TryGetCore(string modId, EnumAppSide side, out ICoreGantryAPI? core)
    {
        ICoreGantryAPI? result = null;
        var found = side.Invoke(
            () => TryGetClientCore(modId, out result),
            () => TryGetServerCore(modId, out result)
        );
        core = result;
        return found;
    }

    /// <summary>
    ///     Provides access to the <see cref="ICoreGantryAPI"/> for the specified mod ID on the client side.
    /// </summary>
    /// <param name="modId">The mod ID of the mod to retrieve the core for.</param>
    /// <param name="core">When this method returns, contains the <see cref="ICoreGantryAPI"/> instance if found; otherwise, null.</param>
    /// <returns>True if the core was found; otherwise, false.</returns>
    public static bool TryGetClientCore(string modId, out ICoreGantryAPI? core)
        => _client.TryGetValue(modId, out core);

    /// <summary>
    ///     Provides access to the <see cref="ICoreGantryAPI"/> for the specified mod ID on the server side.
    /// </summary>
    /// <param name="modId">The mod ID of the mod to retrieve the core for.</param>
    /// <param name="core">When this method returns, contains the <see cref="ICoreGantryAPI"/> instance if found; otherwise, null.</param>
    /// <returns>True if the core was found; otherwise, false.</returns>
    public static bool TryGetServerCore(string modId, out ICoreGantryAPI? core)
        => _server.TryGetValue(modId, out core);

    /// <summary>
    ///     Performs the specified action for each registered <see cref="ICoreGantryAPI"/> on the given side.
    /// </summary>
    /// <param name="side">The side (client or server) on which to perform the action.</param>
    /// <param name="action">The action to perform for each core.</param>
    public static void ForEach(EnumAppSide side, Action<ICoreGantryAPI> action)
    {
        var cores = side.Invoke(() => _client.Values, () => _server.Values);
        if (cores is null) return;
        foreach (var core in cores) action(core);
    }
}