using CommandLine;

namespace Gantry.Tools.ModPackager;

/// <summary>
///     The command line arguments specific to the mod packager, including project, mod, solution, and configuration information.
/// </summary>
public partial class CommandLineArgs : ModInfoFileGenerator.CommandLineArgs
{
    /// <summary>
    ///     The directory where the project files are located.
    /// </summary>
    [Option(nameof(ProjectDir),
        Required = true,
        MetaValue = "$(ProjectDir)",
        HelpText = "The directory where the project files are located. In most cases, should be $(ProjectDir) in MSBuild Macros.")]
    public required string ProjectDir { get; set; }

    /// <summary>
    ///     The unique identifier of the mod.
    /// </summary>
    [Option(nameof(ModId),
        Required = true,
        MetaValue = "$(ModId)",
        HelpText = "The unique identifier of the mod. In most cases, should be $(ModId) in MSBuild Macros.")]
    public required string ModId { get; set; }

    /// <summary>
    ///     The root directory of the solution.
    /// </summary>
    [Option(nameof(SolutionDir),
        Required = true,
        MetaValue = "$(SolutionDir)",
        HelpText = "The root directory of the solution. In most cases, should be $(SolutionDir) in MSBuild Macros.")]
    public required string SolutionDir { get; set; }

    /// <summary>
    ///     The configuration to use for the build (e.g., Debug, Release, or Package).
    /// </summary>
    [Option(nameof(Configuration),
        Required = true,
        MetaValue = "$(Configuration)",
        HelpText = "The configuration to use (eg. Debug, Release, or Package). In most cases, should be $(Configuration) in MSBuild Macros.")]
    public required Configuration Configuration { get; set; }
}