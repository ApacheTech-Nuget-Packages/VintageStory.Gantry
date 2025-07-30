namespace Gantry.Tools.TranslationsPatcher.Properties;

/// <summary>
///     Command line options for the translation merger tool.
/// </summary>
public class CommandLineOptions
{
    /// <summary>
    ///     The root directory of the mod project.
    /// </summary>
    [Option(nameof(ProjectDir), 
        Required = true, 
        HelpText = "The root directory of the mod project. MSBuild: $(ProjectDir)")]
    public string ProjectDir { get; set; } = string.Empty;

    /// <summary>
    ///     The output directory for merged translations.
    /// </summary>
    [Option(nameof(TargetDir), 
        Required = true, 
        HelpText = "The output directory for merged translations. MSBuild: $(TargetDir)")]
    public string TargetDir { get; set; } = string.Empty;

    /// <summary>
    ///     The mod ID.
    /// </summary>
    [Option(nameof(ModId), 
        Required = true, 
        HelpText = "The mod ID.")]
    public string ModId { get; set; } = string.Empty;

    /// <summary>
    ///     The minimum Serilog logging level (e.g. Verbose, Debug, Information, Warning, Error, Fatal). Default is Debug.
    /// </summary>
    [Option(nameof(LogLevel), 
        Required = false, 
        HelpText = "The minimum Serilog logging level (Verbose, Debug, Information, Warning, Error, Fatal). Default: Debug.")]
    public Serilog.Events.LogEventLevel LogLevel { get; set; } = Serilog.Events.LogEventLevel.Debug;
}