using CommandLine;
using Gantry.Tools.ModPackager;
using Gantry.Tools.ModPackager.Steps;
using Serilog;

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
        args.CleanupDebugDir();
    }
    else
    {
        args.CopyFilesFromTargetDirToDebugDir();
        args.CleanupDebugDir();
        args.BackupUnmergedModAssembly();
        var saProject = args.CreateDebugSmartAssemblyProject(out var mergedAssemblies);
        saProject.GenerateSmartAssemblyProjectFile();
        saProject.RunSmartAssemblyProjectFile(args);
        args.CreateModArchive(modDetails, mergedAssemblies);
    }
});

Log.CloseAndFlush();