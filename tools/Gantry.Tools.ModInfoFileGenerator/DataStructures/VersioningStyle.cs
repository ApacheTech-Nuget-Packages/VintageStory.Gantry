namespace Gantry.Tools.ModInfoFileGenerator.DataStructures;

/// <summary>
///     Specifies the strategy for determining the mod version in modinfo.json files.
/// </summary>
public enum VersioningStyle
{
    /// <summary>
    ///     Uses a static version string specified by the <see cref="ModInfoAttribute"/>.
    /// </summary>
    ModInfo,

    /// <summary>
    ///     Uses the version extracted from the assembly's metadata.
    /// </summary>
    Assembly,

    /// <summary>
    ///    User-defined version string, allowing for custom versioning logic or formats.
    /// </summary>
    Custom
}