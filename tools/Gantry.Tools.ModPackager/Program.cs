using CommandLine;
using Gantry.Tools.ModPackager;
using Gantry.Tools.ModPackager.Steps;
using Serilog;
using System.Diagnostics;

var parsedResult = Parser.Default.ParseArguments<CommandLineArgs>(args);
await parsedResult.WithParsedAsync(async args =>
{
    Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Is(args.LogLevel)
        .WriteTo.Console()
        .CreateLogger();


    var modDetails = await args.PrepareModAsync();
    if (args.Configuration == Configuration.Debug)
    {
        args.CopyFilesFromTargetDirToDebugDir();
        var debugDir = args.DebugDir();
        var assemblyPath = Path.Combine(debugDir, args.AssemblyFileName());
        var assemblyFile = new FileInfo(assemblyPath);
        var assemblyDependencies = assemblyFile.GetMergedAssemblies(args);
        args.CleanupDebugDir(assemblyDependencies);
    }
    else
    {
        args.CopyFilesFromTargetDirToDebugDir();
        args.BackupUnmergedModAssembly();
        var saProject = args.CreateDebugSmartAssemblyProject(out var mergedAssemblies);
        args.CleanupDebugDir(mergedAssemblies);
        saProject.GenerateSmartAssemblyProjectFile();
        saProject.RunSmartAssemblyProjectFile(args);
        args.CreateModArchive(modDetails, mergedAssemblies);
    }
});

Log.CloseAndFlush();