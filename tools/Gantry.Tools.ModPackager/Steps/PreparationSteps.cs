using Gantry.Tools.ModInfoFileGenerator.DataStructures;
using Gantry.Tools.TranslationsPatcher;
using Gantry.Tools.TranslationsPatcher.Properties;
using Serilog;
using MIG = Gantry.Tools.ModInfoFileGenerator.Extensions.ModInfoFileGenerator;

namespace Gantry.Tools.ModPackager.Steps;

/// <summary>
///     Provides preparation steps for packaging a mod, including generating modinfo files,
///     copying required files, and patching translations.
/// </summary>
public static class PreparationSteps
{
    private static readonly ILogger _logger = Log.Logger.ForContext(typeof(PreparationSteps));

    public static void LogAllPropertiesInArgs(this CommandLineArgs args)
    {
        Log.Verbose("Logging all properties in CommandLineArgs:");
        Log.Verbose(" - TargetPath: {TargetPath}", args.TargetPath);
        Log.Verbose(" - TargetDir: {TargetDir}", args.TargetDir);
        Log.Verbose(" - DependenciesDir: {DependenciesDir}", args.DependenciesDir);
        Log.Verbose(" - VersioningStyle: {VersioningStyle}", args.VersioningStyle);
        Log.Verbose(" - Version: {Version}", args.Version);
        Log.Verbose(" - LogLevel: {LogLevel}", args.LogLevel);
        Log.Verbose(" --- ");
        Log.Verbose(" - ProjectDir: {ProjectDir}", args.ProjectDir);
        Log.Verbose(" - ModId: {ModId}", args.ModId);
        Log.Verbose(" - SolutionDir: {SolutionDir}", args.SolutionDir);
        Log.Verbose(" - Configuration: {Configuration}", args.Configuration);
        Log.Verbose(" --- ");
        Log.Verbose(" - UnmergedDirName: {UnmergedDirName}", Constants.UnmergedDirName);
        Log.Verbose(" - GantryDirName: {GantryDirName}", Constants.GantryDirName);
        Log.Verbose(" - DebugDirName: {DebugDirName}", Constants.DebugDirName);
        Log.Verbose(" - ReleaseDirName: {ReleaseDirName}", Constants.ReleaseDirName);
        Log.Verbose(" - TempDirName: {TempDirName}", Constants.TempDirName);
        Log.Verbose(" - IncludesDirName: {IncludesDirName}", Constants.IncludesDirName);
        Log.Verbose(" - TranslationsDirName: {TranslationsDirName}", Constants.TranslationsDirName);
        Log.Verbose(" - ModInfoFileName: {ModInfoFileName}", Constants.ModInfoFileName);
        Log.Verbose(" --- ");
        Log.Verbose(" - GantryDir: {GantryDir}", args.GantryDir());
        Log.Verbose(" - ProjectName: {ProjectName}", args.ProjectName());
        Log.Verbose(" - DebugDir: {DebugDir}", args.DebugDir());
        Log.Verbose(" - ReleaseDir: {ReleaseDir}", args.ReleaseDir());
        Log.Verbose(" - TempDir: {TempDir}", args.TempDir());
        Log.Verbose(" - TargetIncludesDir: {TargetIncludesDir}", args.TargetIncludesDir());
        Log.Verbose(" - ModInfoFilePath: {ModInfoFilePath}", args.ModInfoFilePath());
        Log.Verbose(" - AssemblyFileName: {AssemblyFileName}", args.AssemblyFileName());
        Log.Verbose(" - UnmergedDir: {UnmergedDir}", args.UnmergedDir());
        Log.Verbose(" - UnmergedAssemblyFilePath : {UnmergedAssemblyFilePath}", args.UnmergedAssemblyFilePath());
        Log.Verbose(" --- ");
    }

    /// <summary>
    ///     Executes all preparation steps required for packaging a mod.
    ///     This includes generating the modinfo.json file, copying Gantry files,
    ///     copying all _Includes folders, and patching translations into the target directory.
    /// </summary>
    /// <param name="args">The command line arguments containing configuration and directory information for the mod packaging process.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task<ModDetails> PrepareModAsync(this CommandLineArgs args)
    {
        _logger.Information("Starting PrepareModAsync for mod: {ModId}", args.ModId);
        try
        {
            args.LogAllPropertiesInArgs();
            var modDetails = await args.GenerateModInfoFileAsync();
            args.CopyGantryFilesToTargetDir();
            args.CopyProjectIncludesToTargetDir();
            await args.PatchTranslationsAsync();
            _logger.Information("Completed PrepareModAsync for mod: {ModId}", args.ModId);
            return modDetails;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error in PrepareModAsync for mod: {ModId}", args.ModId);
            throw;
        }
    }

    /// <summary>
    ///     Generates a modinfo.json file for the mod in the specified target directory.
    /// </summary>
    /// <param name="args">The command line arguments containing configuration for mod info generation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task<ModDetails> GenerateModInfoFileAsync(this CommandLineArgs args)
    {
        _logger.Information("Generating modinfo.json for mod: {ModId}", args.ModId);
        try
        {
            var modDetails = await MIG.GenerateModInfoFileAsync(args);
            _logger.Information("Generated modinfo.json for mod: {ModId}", args.ModId);
            return modDetails;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error generating modinfo.json for mod: {ModId}", args.ModId);
            throw;
        }
    }

    /// <summary>
    ///     Copies all Gantry files from the solution's .gantry directory to the target directory.
    /// </summary>
    /// <param name="args">The command line arguments containing source and target directory information.</param>
    /// <exception cref="DirectoryNotFoundException">Thrown if the source directory does not exist.</exception>
    public static void CopyGantryFilesToTargetDir(this CommandLineArgs args)
    {
        _logger.Information("Copying Gantry files to target directory for mod: {ModId}", args.ModId);
        var sourceDir = args.GantryDir();
        var targetDir = args.TargetDir;
        if (!Directory.Exists(sourceDir))
        {
            _logger.Error("Source directory '{SourceDir}' does not exist.", sourceDir);
            throw new DirectoryNotFoundException($"Source directory '{sourceDir}' does not exist.");
        }

        foreach (var file in Directory.GetFiles(sourceDir, "*", SearchOption.AllDirectories))
        {
            var relativePath = Path.GetRelativePath(sourceDir, file);
            var targetFile = Path.Combine(targetDir, relativePath);
            Directory.CreateDirectory(Path.GetDirectoryName(targetFile)!);
            File.Copy(file, targetFile, true);
            _logger.Debug("Copied Gantry file: {File}", file);
        }
        _logger.Information("Copied all Gantry files for mod: {ModId}", args.ModId);
    }

    /// <summary>
    ///     Recursively copies all _Includes folders from the project directory into the target directory's _Includes folder.
    /// </summary>
    /// <param name="args">The command line arguments containing project and target directory information.</param>
    /// <exception cref="DirectoryNotFoundException">Thrown if no _Includes directories are found in the project directory.</exception>
    public static void CopyProjectIncludesToTargetDir(this CommandLineArgs args)
    {
        _logger.Information("Copying _Includes folders for mod: {ModId}", args.ModId);

        // Collate all directories named _Includes from the project directory, avoiding the bin and obj directories.
        var includesDirs = Directory.GetDirectories(args.ProjectDir, Constants.IncludesDirName, SearchOption.AllDirectories)
            .Where(dir => !dir.Contains("bin") && !dir.Contains("obj")).ToList();

        foreach (var includesDir in includesDirs)
        {
            var targetIncludesDir = args.TargetIncludesDir();
            RecursiveCopy(includesDir, targetIncludesDir);
            _logger.Debug(" - Copied _Includes directory: {IncludesDir}", includesDir);
        }

        _logger.Information("Copied all _Includes folders for mod: {ModId}", args.ModId);
    }

    private static void RecursiveCopy(string sourceDirectory, string targetDirectory)
    {
        Directory.CreateDirectory(targetDirectory);
        foreach (var file in Directory.GetFiles(sourceDirectory))
        {
            var targetFile = Path.Combine(targetDirectory, Path.GetFileName(file));
            File.Copy(file, targetFile, true);
        }
        foreach (var subDir in Directory.GetDirectories(sourceDirectory))
        {
            var targetSubDir = Path.Combine(targetDirectory, Path.GetFileName(subDir));
            RecursiveCopy(subDir, targetSubDir);
        }
    }

    /// <summary>
    ///     Patches and merges translation files into the target directory's _Includes folder.
    /// </summary>
    /// <param name="args">The command line arguments containing project, target, and mod information.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task PatchTranslationsAsync(this CommandLineArgs args)
    {
        _logger.Information("Patching translations for mod: {ModId}", args.ModId);
        try
        {
            await new CommandLineOptions
            {
                ProjectDir = args.ProjectDir,
                TargetDir = args.TargetDir,
                ModId = args.ModId,
                LogLevel = args.LogLevel
            }.PatchTranslationFilesAsync();
            _logger.Information("Patched translations for mod: {ModId}", args.ModId);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error patching translations for mod: {ModId}", args.ModId);
            throw;
        }
    }
}