namespace Gantry.Tools.ModInfoFileGenerator.DataStructures;

/// <summary>
///     Specifies the type of mod, used for categorising and serialising mod metadata.
/// </summary>
public enum ModType
{
    /// <summary>
    ///     A mod that contains executable code (e.g., DLLs).
    /// </summary>
    Code,

    /// <summary>
    ///     A mod that provides additional content, such as assets or data files.
    /// </summary>
    Content,

    /// <summary>
    ///     A mod that includes source code for reference or development purposes.
    /// </summary>
    Source,

    /// <summary>
    ///     A mod that provides a theme or visual style.
    /// </summary>
    Theme
}