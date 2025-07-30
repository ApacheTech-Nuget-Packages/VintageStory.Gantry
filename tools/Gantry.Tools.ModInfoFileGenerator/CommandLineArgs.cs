using Gantry.Tools.ModInfoFileGenerator.DataStructures;
using Serilog.Events;

namespace Gantry.Tools.ModInfoFileGenerator;

/// <summary>
///     Represents the command line arguments for the mod info file generator packager.
///     Used to specify the target assembly, output directory, and versioning mode.
/// </summary>
public class CommandLineArgs
{
    /// <summary>
    ///     The path to the target assembly. In most cases, this should be $(TargetPath) in MSBuild Macros.
    /// </summary>
    [Option(nameof(TargetPath),
        Required = true,
        MetaValue = "$(TargetPath)",
        HelpText = "The target assembly. In most cases, should be $(TargetPath) in MSBuild Macros.")]
    public required string TargetPath { get; set; }

    /// <summary>
    ///     The output directory for the generated modinfo.json file. In most cases, this should be $(TargetDir) in MSBuild Macros.
    /// </summary>
    [Option(nameof(TargetDir),
        Required = true,
        MetaValue = "$(TargetDir)",
        HelpText = "The output directory. In most cases, should be $(TargetDir) in MSBuild Macros.")]
    public required string TargetDir { get; set; }

    /// <summary>
    ///     The dependencies directory. In most cases, should be $(SolutionDir).debug in MSBuild Macros.
    /// </summary>
    [Option(nameof(DependenciesDir),
        Required = false,
        MetaValue = "$(SolutionDir).debug",
        HelpText = "The dependencies directory. In most cases, should be $(SolutionDir).debug in MSBuild Macros.")]
    public required string DependenciesDir { get; set; }

    /// <summary>
    ///     The versioning style. [static] will take the version from the ModInfoAttribute. [assembly] will use the version of the assembly itself.
    /// </summary>
    [Option(nameof(VersioningStyle),
        Required = false,
        Default = VersioningStyle.ModInfo,
        MetaValue = "ModInfo|Assembly|Custom",
        HelpText = "The target version. [ModInfo] will take the version from the ModInfoAttribute. [Assembly] will use the version of the assembly itself. [Custom]")]
    public VersioningStyle VersioningStyle { get; set; } = VersioningStyle.ModInfo;

    /// <summary>
    ///     When using the Custom versioning style, this is the version string to use.
    /// </summary>
    [Option(nameof(Version),
        Required = false,
        MetaValue = "1.0.0",
        HelpText = "The version to use when using the Custom versioning style.")]
    public string? Version { get; set; }

    /// <summary>
    ///     The verbosity level for logging. Defaults to Information.
    /// </summary>
    [Option(nameof(LogLevel),
        Required = false,
        Default = LogEventLevel.Information,
        MetaValue = "Verbose|Debug|Information|Warning|Error|Fatal",
        HelpText = "The verbosity level for logging. Defaults to Information.")]
    public LogEventLevel LogLevel { get; set; } = LogEventLevel.Information;
}