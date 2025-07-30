namespace Gantry.GameContent.ChatCommands.DataStructures;

/// <summary>
///     The mode of access for specific feature.
/// </summary>
public enum AccessMode
{
    /// <summary>
    ///     The feature is disabled for all players on the server.
    /// </summary>
    Disabled,

    /// <summary>
    ///     The feature is enabled for all players on the server.
    /// </summary>
    Enabled,

    /// <summary>
    ///     The feature is only enabled for players on the whitelist.
    /// </summary>
    Whitelist,

    /// <summary>
    ///     The feature is enabled all for players, except for player on the blacklist.
    /// </summary>
    Blacklist
}