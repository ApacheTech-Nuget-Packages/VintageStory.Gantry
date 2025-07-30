namespace Gantry.Tools.ModInfoFileGenerator.DataStructures;

/// <summary>
///     Represents metadata details for a mod.
/// </summary>
public record ModDetails
{
    /// <summary>
    ///     The determined version of the mod.
    /// </summary>
    public required string Version { get; init; }

    /// <summary>
    ///     The <see cref="ModInfoAttribute"/> of the mod.
    /// </summary>
    public required ModInfoAttribute ModInfo { get; init; }

    /// <summary>
    ///     Indicates whether the mod was built in debug configuration.
    /// </summary>
    public required bool DebugConfiguration { get; init; }

    /// <summary>
    ///     The file system location of the mod assembly.
    /// </summary>
    public required string Location { get; init; }

    /// <summary>
    ///     The name of the mod assembly file.
    /// </summary>
    public required string AssemblyName { get; init; }

    /// <summary>
    ///     The list of dependencies required by the mod.
    /// </summary>
    public required IReadOnlyDictionary<string, string> Dependencies { get; init; }
}