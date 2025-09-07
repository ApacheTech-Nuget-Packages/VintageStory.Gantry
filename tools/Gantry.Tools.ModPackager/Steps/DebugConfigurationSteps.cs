using Gantry.Tools.ModPackager.SmartAssembly;
using Serilog;

namespace Gantry.Tools.ModPackager.Steps;

/// <summary>
///     Provides steps for handling debug configuration tasks, including copying files to the debug directory,
///     backing up unmerged assemblies, creating SmartAssembly project files, and cleaning up debug directories.
/// </summary>
public static class DebugConfigurationSteps
{
    private static readonly ILogger _logger = Log.Logger.ForContext(typeof(DebugConfigurationSteps));

    #region Copy Files

    /// <summary>
    ///     Copies all files from the target directory to the debug directory for the current project.
    /// </summary>
    /// <param name="args">The command line arguments containing directory information.</param>
    public static void CopyFilesFromTargetDirToDebugDir(this CommandLineArgs args)
    {
        _logger.Information("Copying files to debug directory for project: {ProjectName}", args.ProjectName());
        try
        {
            var targetDir = args.TargetDir;
            var debugDir = args.DebugDir();
            Directory.CreateDirectory(debugDir);
            foreach (var file in Directory.GetFiles(targetDir, "*", SearchOption.AllDirectories))
            {
                var relativePath = Path.GetRelativePath(targetDir, file);
                var targetFile = Path.Combine(debugDir, relativePath);
                Directory.CreateDirectory(Path.GetDirectoryName(targetFile)!);
                File.Copy(file, targetFile, true);
                _logger.Debug("Copied file to debug dir: {File}", file);
            }
            _logger.Information("Copied all files to debug directory: {DebugDir}", debugDir);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error copying files to debug directory for project: {ProjectDir}", args.ProjectDir);
            throw;
        }
    }

    #endregion

    #region Smart Assembly

    /// <summary>
    ///     Backs up the unmerged mod assembly by copying it to the unmerged directory.
    /// </summary>
    /// <param name="args">The command line arguments containing directory and file information.</param>
    public static void BackupUnmergedModAssembly(this CommandLineArgs args)
    {
        _logger.Information("Backing up unmerged mod assembly for project: {ProjectDir}", args.ProjectDir);
        try
        {
            var sourceDll = Path.Combine(args.DebugDir(), args.AssemblyFileName());
            var targetDll = args.UnmergedAssemblyFilePath();
            Directory.CreateDirectory(args.UnmergedDir());
            File.Copy(sourceDll, targetDll, true);

            var depsFile = Path.Combine(args.DebugDir(), $"{args.ProjectName()}.deps.json");
            File.Copy(depsFile, Path.Combine(args.UnmergedDir(), $"{args.ProjectName()}.deps.json"), true);
            _logger.Debug("Backed up unmerged mod assembly and deps file from {SourceDll} to {TargetDll}", sourceDll, targetDll);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error backing up unmerged mod assembly for project: {ProjectDir}", args.ProjectDir);
            throw;
        }
    }

    /// <summary>
    ///     Creates a SmartAssembly project for the debug build, specifying merged assemblies and output paths.
    /// </summary>
    /// <param name="args">The command line arguments containing project and configuration information.</param>
    /// <returns>A <see cref="SmartAssemblyProject"/> instance configured for the debug build.</returns>
    public static SmartAssemblyProject CreateDebugSmartAssemblyProject(this CommandLineArgs args, out List<AssemblyReference> assemblyDependencies)
    {
        _logger.Information("Creating SmartAssembly project for debug build: {ProjectName}", args.ProjectName());
        try
        {
            var debugDir = args.DebugDir();
            var assemblyPath = Path.Combine(debugDir, args.AssemblyFileName());
            var assemblyFile = new FileInfo(assemblyPath);
            assemblyDependencies = assemblyFile.GetMergedAssemblies(args);

            var saprojName = $"{args.ProjectName()}.saproj";
            var saprojPath = Path.Combine(debugDir, saprojName);

            var project = new SmartAssemblyProject
            {
                ProjectFileName = saprojPath,
                MainAssemblyFileName = args.UnmergedAssemblyFilePath(),
                DestinationFileName = assemblyPath,
                FriendlyName = args.ProjectName(),
                Configuration = args.Configuration,
                MergedAssemblies = assemblyDependencies,
            };
            _logger.Information("Created SmartAssembly project file: {saprojName}", saprojName);
            return project;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error creating SmartAssembly project for debug build: {ProjectDir}", args.ProjectDir);
            throw;
        }
    }

    #endregion

    #region Cleanup

    /// <summary>
    ///     Moves all files from the _Includes directory to the debug directory and deletes the _Includes directory.
    /// </summary>
    /// <param name="args">The command line arguments containing debug directory information.</param>
    public static void CleanupDebugDir(this CommandLineArgs args)
    {
        _logger.Information("Cleaning up debug directory for project: {ProjectDir}", args.ProjectDir);
        //args.MoveMergedDependenciesIntoUnmergedDir(mergedAssemblies);
        var debugDir = args.DebugDir();
        var includesDir = Path.Combine(debugDir, Constants.IncludesDirName);
        if (Directory.Exists(includesDir))
        {
            try
            {
                foreach (var file in Directory.GetFiles(includesDir, "*", SearchOption.AllDirectories))
                {
                    var relativePath = Path.GetRelativePath(includesDir, file);
                    var targetFile = Path.Combine(debugDir, relativePath);
                    Directory.CreateDirectory(Path.GetDirectoryName(targetFile)!);
                    File.Move(file, targetFile, true);
                    _logger.Debug("Moved file from _Includes to debug dir: {File}", file);
                }
                Directory.Delete(includesDir, true);
                _logger.Information("Deleted _Includes directory: {IncludesDir}", includesDir);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error cleaning up debug directory for project: {ProjectDir}", args.ProjectDir);
                throw;
            }
        }
        else
        {
            _logger.Warning("No _Includes directory found in debug dir: {IncludesDir}", includesDir);
        }
    }

    #endregion
}