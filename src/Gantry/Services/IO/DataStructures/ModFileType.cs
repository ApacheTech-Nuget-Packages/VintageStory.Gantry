namespace Gantry.Services.IO.DataStructures;

/// <summary>
///     Represents the type of a mod file.
/// </summary>
public enum ModFileType
{
    /// <summary>
    ///     A file that is used to store settings for a mod.
    /// </summary>
    Settings,

    /// <summary>
    ///     A file that is used to store data for a mod.
    /// </summary>
    Data,

    /// <summary>
    ///     A file that is used to store assets for a mod, such as textures, sounds, etc.
    /// </summary>
    Assets
}