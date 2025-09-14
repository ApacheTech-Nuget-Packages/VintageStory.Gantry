namespace Gantry.Tools.ModPackager;

/// <summary>
///     Constant values and extension methods for directory and file naming conventions used in the mod packager.
/// </summary>
public static class Constants
{
    /// <summary>
    ///     Name of the directory for unmerged assemblies.
    /// </summary>
    public const string UnmergedDirName = "unmerged";

    /// <summary>
    ///     Name of the Gantry configuration directory.
    /// </summary>
    public const string GantryDirName = ".gantry";

    /// <summary>
    ///     Name of the debug directory.
    /// </summary>
    public const string DebugDirName = ".debug";

    /// <summary>
    ///     Name of the release directory.
    /// </summary>
    public const string ReleaseDirName = ".releases";

    /// <summary>
    ///     Name of the temporary directory.
    /// </summary>
    public const string TempDirName = ".tmp";

    /// <summary>
    ///     Name of the includes directory.
    /// </summary>
    public const string IncludesDirName = "_Includes";

    /// <summary>
    ///     Name of the translations directory.
    /// </summary>
    public const string TranslationsDirName = "_Translations";

    /// <summary>
    ///     Name of the mod info file.
    /// </summary>
    public const string ModInfoFileName = "modinfo.json";

    /// <summary>
    ///     The expected file name for the Gantry assembly.
    /// </summary>
    public const string GantryAssemblyFileName = "Gantry.dll";

    /// <summary>
    ///     The environment variable that points to the Vanilla installation directory.
    /// </summary>
    public static string VanillaDir() => Environment.GetEnvironmentVariable("VINTAGE_STORY")!;

    /// <summary>
    ///     The directory info for the Lib directory inside the Vanilla installation.
    /// </summary>
    public static DirectoryInfo VanillaLibDir() => new(Path.Combine(VanillaDir(), "Lib"));

    /// <summary>
    ///     Path to the Gantry directory for the current solution.
    /// </summary>
    public static string GantryDir(this CommandLineArgs args) => Path.Combine(args.SolutionDir, GantryDirName);
   
    /// <summary>
    ///     Project name from the project directory.
    /// </summary>
    public static string ProjectName(this CommandLineArgs args) => Path.GetFileNameWithoutExtension(args.TargetPath);

    /// <summary>
    ///     Path to the debug directory for the current project.
    /// </summary>
    public static string DebugDir(this CommandLineArgs args) => Path.Combine(args.SolutionDir, DebugDirName, args.ProjectName());
    
    /// <summary>
    ///     Path to the release directory for the current solution.
    /// </summary>
    public static string ReleaseDir(this CommandLineArgs args) => Path.Combine(args.SolutionDir, ReleaseDirName);
    
    /// <summary>
    ///     Path to the temporary directory for the current solution.
    /// </summary>
    public static string TempDir(this CommandLineArgs args) => Path.Combine(args.SolutionDir, TempDirName);
    
    /// <summary>
    ///     Path to the _Includes directory in the target directory.
    /// </summary>
    public static string TargetIncludesDir(this CommandLineArgs args) => Path.Combine(args.TargetDir, IncludesDirName);
    
    /// <summary>
    ///     Path to the modinfo.json file in the target directory.
    /// </summary>
    public static string ModInfoFilePath(this CommandLineArgs args) => Path.Combine(args.TargetDir, ModInfoFileName);
    
    /// <summary>
    ///     Expected assembly file name for the project.
    /// </summary>
    public static string AssemblyFileName(this CommandLineArgs args) => Path.GetFileName(args.TargetPath);
    
    /// <summary>
    ///     Path to the unmerged directory in the debug directory.
    /// </summary>
    public static string UnmergedDir(this CommandLineArgs args) => Path.Combine(args.SolutionDir, DebugDirName, $"{args.ProjectName()}-{UnmergedDirName}");

    /// <summary>
    ///     Path to the unmerged assembly file in the unmerged directory.
    /// </summary>
    public static string UnmergedAssemblyFilePath(this CommandLineArgs args) => Path.Combine(args.UnmergedDir(), args.AssemblyFileName());
}