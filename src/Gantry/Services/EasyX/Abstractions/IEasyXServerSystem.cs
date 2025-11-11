using Gantry.Services.IO.Configuration.Abstractions;

namespace Gantry.Services.EasyX.Abstractions;

/// <summary>
///     Represents the base settings for each EasyX feature within this mod (server-side).
/// </summary>
/// <typeparam name="TServerSettings"></typeparam>
public interface IEasyXServerSystem<TServerSettings>
    where TServerSettings : FeatureSettings<TServerSettings>, IEasyXServerSettings, new()
{
    /// <summary>
    ///     The server-side settings for this feature.
    /// </summary>
    TServerSettings Settings { get; }

    /// <summary>
    ///     Determines whether the feature is enabled for the specified player.
    /// </summary>
    /// <param name="player">The player to check.</param>
    /// <returns>True if the feature is enabled for the player; otherwise, false.</returns>
    bool IsEnabledFor(IPlayer player);

    /// <summary>
    ///     Determines whether the feature is enabled for all of the specified players.
    /// </summary>
    /// <param name="players">The players to check.</param>
    /// <returns>True if the feature is enabled for all of the players; otherwise, false.</returns>
    bool IsEnabledForAll(IEnumerable<string> players);

    /// <summary>
    ///     Determines whether the feature is enabled for any of the specified players.
    /// </summary>
    /// <param name="players">The players to check.</param>
    /// <returns>True if the feature is enabled for any of the players; otherwise, false.</returns>
    bool IsEnabledForAny(IEnumerable<string> players);
}