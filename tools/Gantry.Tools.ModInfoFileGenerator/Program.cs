using Gantry.Tools.ModInfoFileGenerator;
using Gantry.Tools.ModInfoFileGenerator.Extensions;

await Parser.Default
    .ParseArguments<CommandLineArgs>(args)
    .WithParsedAsync(async args =>
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Is(args.LogLevel)
            .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();

        await args.GenerateModInfoFileAsync();
    });
Log.CloseAndFlush();