await Parser.Default.ParseArguments<CommandLineOptions>(args)
    .WithParsedAsync(async args =>
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Is(args.LogLevel)
            .WriteTo.Console()
            .CreateLogger();

        await args.PatchTranslationFilesAsync();
    });
Log.CloseAndFlush();