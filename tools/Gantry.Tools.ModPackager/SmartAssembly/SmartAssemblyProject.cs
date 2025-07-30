namespace Gantry.Tools.ModPackager.SmartAssembly;

/// <summary>
///     The SmartAssembly project definition, containing configuration and metadata for merging assemblies.
/// </summary>
public record SmartAssemblyProject
{
    /// <summary>
    ///     The file name of the SmartAssembly project file (.saproj).
    /// </summary>
    public required string ProjectFileName { get; init; }

    /// <summary>
    ///     The file name of the main assembly to be processed by SmartAssembly.
    /// </summary>
    public required string MainAssemblyFileName { get; init; }

    /// <summary>
    ///     The file name of the destination (output) assembly after merging.
    /// </summary>
    public required string DestinationFileName { get; init; }

    /// <summary>
    ///     The friendly name of the project or application.
    /// </summary>
    public required string FriendlyName { get; init; }

    /// <summary>
    ///     The build configuration to use for the SmartAssembly project.
    /// </summary>
    public required Configuration Configuration { get; init; }

    /// <summary>
    ///     The list of assemblies to be merged into the main assembly.
    /// </summary>
    public required List<AssemblyReference> MergedAssemblies { get; init; }

    /// <summary>
    ///     The company name to use in the SmartAssembly project.
    /// </summary>
    public static string CompanyName => "ApacheTech Solutions";
}
