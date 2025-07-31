using Gantry.Core.Abstractions;
using Gantry.Extensions.Api;

namespace Gantry.Core;

/// <summary>
///     Provides inter-mod functionality for Gantrified mods.
/// </summary>
public static class Nexus
{
    private static readonly Dictionary<string, ICoreGantryAPI> _client = [];
    private static readonly Dictionary<string, ICoreGantryAPI> _server = [];

    /// <summary>
    ///     Adds the provided <see cref="ICoreGantryAPI"/> to the list of active mods.
    /// </summary>
    /// <param name="api"></param>
    public static void AddCore(ICoreGantryAPI api) 
        => api.Side.Invoke(
            () => _client.Add(api.Mod.Info.ModID, api), 
            () => _server.Add(api.Mod.Info.ModID, api));

    /// <summary>
    ///     Provides access to the <see cref="ICoreGantryAPI"/> for the specified mod ID and side.
    /// </summary>
    /// <param name="modId">The mod ID of the mod to retrieve the core for.</param>
    /// <param name="side">The side (client or server) for which to retrieve the core.</param>
    /// <returns>An instance of <see cref="ICoreGantryAPI"/> for the specified mod ID and side.</returns>
    /// <exception cref="KeyNotFoundException">No core found for the specified mod ID and side.</exception>
    public static ICoreGantryAPI GetCore(string modId, EnumAppSide side) 
        => side.Invoke(
            () => _client.TryGetValue(modId, out var api) ? api! : null,
            () => _server.TryGetValue(modId, out var api) ? api! : null) 
                ?? throw new KeyNotFoundException($"No core found for mod ID '{modId}' on side '{side}'.");
}