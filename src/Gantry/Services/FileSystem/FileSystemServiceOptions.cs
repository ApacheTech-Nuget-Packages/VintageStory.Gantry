

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

namespace Gantry.Services.FileSystem;

/// <summary>
/// Configuration options for the file system service.
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class FileSystemServiceOptions
{
    /// <summary>
    ///     Gets the default settings for the file system service. Sets the root folder name to the Mod ID.
    /// </summary>
    public static FileSystemServiceOptions Default { get; } = new();

    /// <summary>
    ///     Returns the name of the root folder to use to store files for this mod, within the data folder of the game.
    ///     %VINTAGE_STORY_DATA%\ModData\{RootFolderName}\
    /// </summary>
    /// <value>
    ///     The name of the root folder to use to store files for this mod.
    /// </value>
    public string RootFolderName { get; set; } = ModEx.ModInfo.ModID ?? Guid.NewGuid().ToString();

    /// <summary>
    ///     Determines whether to register the standard settings files for the mod. Default: False.
    /// </summary>
    /// <remarks>
    ///     "%VINTAGE_STORY_DATA%\ModData\{RootFolderName}\{Scope}\settings-{scope}-{side}.json"
    /// </remarks>
    public bool RegisterSettingsFiles { get; set; }
}