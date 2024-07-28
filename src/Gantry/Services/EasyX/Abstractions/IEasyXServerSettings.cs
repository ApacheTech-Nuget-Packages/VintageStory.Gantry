﻿using Gantry.Services.EasyX.ChatCommands.DataStructures;
using Gantry.Services.FileSystem.Configuration.Abstractions;
using JetBrains.Annotations;

namespace Gantry.Services.EasyX.Abstractions;

/// <summary>
///     Represents the base settings for each EasyX feature within this mod.
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.All)]
public interface IEasyXServerSettings<TSettings>
{
    /// <summary>
    ///     Determines whether the feature should be used.
    /// </summary>
    AccessMode Mode { get; set; }

    /// <summary>
    ///     When the mode is set to `Whitelist`, only the players on this list will have the feature enabled.
    /// </summary>
    List<Player> Whitelist { get; set; }

    /// <summary>
    ///     When the mode is set to `Blacklist`, the players on this list will have the feature disabled.
    /// </summary>
    List<Player> Blacklist { get; set; }
}