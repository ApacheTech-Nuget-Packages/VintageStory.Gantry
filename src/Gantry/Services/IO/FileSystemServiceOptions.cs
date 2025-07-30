namespace Gantry.Services.IO;

/// <summary>
///     Configuration options for the file system service.
/// </summary>
public class FileSystemServiceOptions
{
    /// <summary>
    ///     Gets the default settings for the file system service. Sets the root folder name to the Mod ID.
    /// </summary>
    public static FileSystemServiceOptions Default { get; } = new();

    /// <summary>
    ///     Determines whether to register the standard settings files for the mod. Default: True.
    /// </summary>
    /// <remarks>
    ///     "%VINTAGE_STORY_DATA%\ModConfig\{ModId}\{Scope}\settings-{scope}-{side}.json"
    /// </remarks>
    public bool RegisterSettingsFiles { get; set; } = true;
}