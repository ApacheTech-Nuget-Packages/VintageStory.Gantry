using Gantry.Core.GameContent.ChatCommands.DataStructures;

namespace Gantry.Services.EasyX.Abstractions;

/// <summary>
///     Represents the base settings for each EasyX feature within this mod.
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.All)]
public interface IEasyXServerSettings
{
    /// <summary>
    ///     Determines whether the feature should be used.
    /// </summary>
    [DoNotPrune]
    AccessMode Mode { get; set; }

    /// <summary>
    ///     When the mode is set to `Whitelist`, only the players on this list will have the feature enabled.
    /// </summary>
    [DoNotPrune]
    List<Player> Whitelist { get; set; }

    /// <summary>
    ///     When the mode is set to `Blacklist`, the players on this list will have the feature disabled.
    /// </summary>
    [DoNotPrune]
    List<Player> Blacklist { get; set; }
}