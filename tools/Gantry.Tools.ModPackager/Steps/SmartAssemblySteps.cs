using Gantry.Tools.Common.ModLoader;
using Gantry.Tools.ModPackager.SmartAssembly;
using System.Diagnostics;
using Serilog;
using System.IO.Compression;
using Gantry.Tools.ModInfoFileGenerator.DataStructures;

namespace Gantry.Tools.ModPackager.Steps;

/// <summary>
///     The steps for generating, running, and analyzing SmartAssembly project files as part of the mod packaging process.
/// </summary>
public static class SmartAssemblySteps
{
    private static readonly ILogger _logger = Log.Logger.ForContext(typeof(SmartAssemblySteps));

    /// <summary>
    ///     Generates a SmartAssembly project file for the specified project, including merged assemblies and configuration.
    /// </summary>
    /// <param name="project">The SmartAssembly project definition containing file names and configuration.</param>
    public static void GenerateSmartAssemblyProjectFile(this SmartAssemblyProject project)
    {
        _logger.Information("Generating SmartAssembly project file: {ProjectFile}", project.ProjectFileName);
        try
        {
            var mergedAssemblies = string.Join(Environment.NewLine, project.MergedAssemblies.Select(a => a.ToString()));
            var saprojContent = $$"""
            <SmartAssemblyProject ProjectId="{{{Guid.NewGuid()}}}" Version="2.0">
                <MainAssemblyFileName>{{project.MainAssemblyFileName}}</MainAssemblyFileName>                        
                <FriendlyName>{{project.FriendlyName}}</FriendlyName>
                <CompanyName>{{SmartAssemblyProject.CompanyName}}</CompanyName>                
                <Configuration Name="{{project.Configuration}}">
                    <Options>
            	        <ExceptionReporting ReportExceptions="0" Template="res:{SmartExceptions}.NoUI.dll" />
            	        <FeatureUsageReporting Template="res:SmartUsageWithoutUI.dll" />
            	        <StringsEncoding Compress="1" Encode="0" UseCache="1" UseImprovedEncoding="1" />
            	        <OtherProtections SuppressIldasm="1" />
            	        <OtherOptimizations SealClasses="0" />
            	        <Debugging CreatePDB="1" />
            	        <CopyDependencies />
            	        <StrongNameSigning Sign="0" />
                    </Options>
                    <ApplicationName>{{project.FriendlyName}}</ApplicationName>
                    <Destination DestinationFileName="{{project.DestinationFileName}}" />
                    <Assemblies>
                        {{mergedAssemblies}}
                    </Assemblies>
                </Configuration>
            </SmartAssemblyProject>
            """;
            File.WriteAllText(project.ProjectFileName, saprojContent);
            _logger.Information("SmartAssembly project file generated: \n{ProjectFile}", project.ProjectFileName);
            _logger.Verbose("{Xml}", saprojContent);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error generating SmartAssembly project file: {ProjectFile}", project.ProjectFileName);
            throw;
        }
    }

    /// <summary>
    ///     Runs the SmartAssembly process using the specified project file, waits for completion, and deletes the project file.
    /// </summary>
    /// <param name="project">The SmartAssembly project definition containing the project file path.</param>
    /// <exception cref="FileNotFoundException">Thrown if the project file does not exist.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the SA_CONSOLE environment variable is not set or the process fails to start.</exception>
    public static void RunSmartAssemblyProjectFile(this SmartAssemblyProject project, CommandLineArgs args)
    {
        _logger.Information("Running SmartAssembly project file: {ProjectFile}", project.ProjectFileName);
        var saprojFile = project.ProjectFileName;
        if (!File.Exists(saprojFile))
        {
            _logger.Error("SmartAssembly project file '{ProjectFile}' not found.", saprojFile);
            throw new FileNotFoundException($"SmartAssembly project file '{saprojFile}' not found.");
        }
        var saConsoleApp = Environment.GetEnvironmentVariable("SA_CONSOLE");
        if (string.IsNullOrEmpty(saConsoleApp))
        {
            _logger.Error("SA_CONSOLE environment variable is not set.");
            throw new InvalidOperationException("SA_CONSOLE environment variable is not set.");
        }
        try
        {
            using var process = Process.Start(saConsoleApp, saprojFile)
                ?? throw new InvalidOperationException("Failed to start SmartAssembly process.");
            _logger.Debug("SmartAssembly process started for: {ProjectFile}", saprojFile);
            process.WaitForExit();
            _logger.Information("SmartAssembly process completed for: {ProjectFile}", saprojFile);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error running SmartAssembly process for: {ProjectFile}", saprojFile);
            throw;
        }
        finally
        {
            try
            {
                File.Delete(saprojFile);
                _logger.Debug("Deleted SmartAssembly project file: {ProjectFile}", saprojFile);
            }
            catch (Exception ex)
            {
                _logger.Warning(ex, "Failed to delete SmartAssembly project file: {ProjectFile}", saprojFile);
            }
        }
    }

    /// <summary>
    ///     Returns a list of referenced assemblies to be merged, filtered by prefix, for the specified assembly file.
    ///     This method recursively populates the list with transitive dependencies, avoiding duplicates and infinite loops.
    /// </summary>
    /// <param name="assemblyFile">The assembly file to analyze for references.</param>
    /// <param name="args">The command line arguments containing dependency directory information.</param>
    /// <param name="assemblyReferences">The list of <see cref="AssemblyReference"/> objects to be merged. If null, a new list is created.</param>
    /// <param name="visited">A set of already visited assembly names to avoid processing duplicates and infinite loops. Used internally for recursion.</param>
    /// <returns>The list of <see cref="AssemblyReference"/> objects to be merged, including transitive dependencies.</returns>
    public static List<AssemblyReference> GetMergedAssemblies(this FileInfo assemblyFile, CommandLineArgs args, List<AssemblyReference>? assemblyReferences = default, HashSet<string>? visited = null)
    {
        _logger.Information("Getting merged assemblies for: {AssemblyFile}", assemblyFile.FullName);
        try
        {
            visited ??= new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            assemblyReferences ??= [];
            if (!visited.Add(assemblyFile.Name))
            {
                _logger.Debug("Already visited assembly file: {AssemblyFile}", assemblyFile.FullName);
                return assemblyReferences;
            }
            var dependencies = assemblyFile.WithAssemblyContext(args.DependenciesDir, assembly =>
            {
                string[] includedPrefixes = ["Gantry", "ApacheTech"];
                var result = assembly.GetReferencedAssemblies()
                    .Where(ra => ra.Name is not null)
                    .Where(ra => includedPrefixes.Any(prefix => ra.Name!.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)))
                    .Select(ra => new AssemblyReference(ra.Name!))
                    .ToList();
                _logger.Debug("Found {Count} merged assemblies for: {AssemblyFile}", result.Count, assemblyFile.FullName);
                return result;
            });
            if (dependencies is null) return assemblyReferences;
            foreach (var dependency in dependencies)
            {
                if (assemblyReferences.Any(a => a.Name.Equals(dependency.Name, StringComparison.OrdinalIgnoreCase)))
                {
                    _logger.Debug("Skipping already included assembly: {AssemblyName}", dependency.Name);
                    continue;
                }
                _logger.Debug("Adding merged assembly: {AssemblyName}", dependency.Name);
                assemblyReferences.Add(dependency);
                var dependencyFilePath = Path.Combine(args.DependenciesDir, dependency.Name + ".dll");
                if (File.Exists(dependencyFilePath))
                {
                    new FileInfo(dependencyFilePath).GetMergedAssemblies(args, assemblyReferences, visited);
                }
                else
                {
                    _logger.Debug("Dependency file not found for: {DependencyName}", dependency.Name);
                }
            }
            return assemblyReferences;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error getting merged assemblies for: {AssemblyFile}", assemblyFile.FullName);
            throw;
        }
    }

    public static void MoveMergedDependenciesIntoUnmergedDir(this CommandLineArgs args, List<AssemblyReference> mergedAssemblies)
    {
        var sourceDir = args.DebugDir();
        _logger.Debug("Moving merged dependencies from {SourceDir} to unmerged directory", sourceDir);
        foreach (var assembly in mergedAssemblies)
        {
            if (assembly.Name is null)
            {
                _logger.Debug("Assembly name is null, skipping: {Assembly}", assembly);
                continue;
            }
            var assemblyName = assembly.Name;

            Directory.EnumerateFiles(sourceDir, $"{assemblyName}.*", SearchOption.TopDirectoryOnly)
                .Select(f => new FileInfo(f)).ToList().ForEach(f =>
                {
                    if (!f.Exists)
                    {
                        _logger.Warning("File does not exist: {File}", f.FullName);
                        return;
                    }
                    var destinationPath = Path.Combine(args.UnmergedDir(), f.Name);
                    _logger.Debug("Moving merged assembly: {AssemblyName}", f.Name);
                    f.MoveTo(destinationPath);
                    _logger.Verbose("Moved {AssemblyName}", f.Name);
                });
        }
    }

    public static void CreateModArchive(this CommandLineArgs args, ModDetails details)
    {
        var sourceDir = args.DebugDir();
        var destinationDir = args.ReleaseDir();
        _logger.Information("Creating mod archive from {SourceDir} to {DestinationDir}", sourceDir, destinationDir);
        try
        {
            if (!Directory.Exists(sourceDir))
            {
                _logger.Error("Source directory does not exist: {SourceDir}", sourceDir);
                throw new DirectoryNotFoundException($"Source directory '{sourceDir}' does not exist.");
            }
            if (!Directory.Exists(destinationDir))
            {
                Directory.CreateDirectory(destinationDir);
                _logger.Information("Created destination directory: {DestinationDir}", destinationDir);
            }
            var configurationSuffix = args.Configuration == Configuration.Package 
                ? string.Empty 
                : $"-{args.Configuration.ToString().ToUpperInvariant()}";

            var archiveFileName = Path.Combine(destinationDir, $"{args.ProjectName()}_v{details.Version}{configurationSuffix}.zip");
            using var archive = ZipFile.Open(archiveFileName, ZipArchiveMode.Create);
            foreach (var file in Directory.EnumerateFiles(sourceDir, "*.*", SearchOption.AllDirectories))
            {
                var entryName = Path.GetRelativePath(sourceDir, file);
                archive.CreateEntryFromFile(file, entryName);
                _logger.Debug("Added file to archive: {File}", file);
            }
            _logger.Information("Mod archive created successfully: {ArchiveFile}", archiveFileName);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error creating mod archive from {SourceDir} to {DestinationDir}", sourceDir, destinationDir);
            throw;
        }
    }
}
