using Gantry.Core;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

namespace Gantry.Services.Network;

/// <summary>
///     Configuration options for the file system service.
/// </summary>
public class NetworkServiceOptions
{
    /// <summary>
    ///     Gets the default settings for the file system service. Sets the root folder name to the Mod ID.
    /// </summary>
    public static NetworkServiceOptions Default { get; } = new();

    /// <summary>
    ///     Returns the name used to create the default network channel for the mod. Defaults as the ModID.
    /// </summary>
    /// <value>
    ///     The name of the root folder to use to store files for this mod.
    /// </value>
    public string DefaultChannelName { get; set; } = ModEx.ModInfo.ModID;
}