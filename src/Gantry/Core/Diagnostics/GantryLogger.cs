using Vintagestory;

namespace Gantry.Core.Diagnostics;

/// <summary>
///     A logger implementation for the Gantry mod, providing customised log handling and formatting.
/// </summary>
public abstract class GantryLogger : Logger
{
    private readonly EnumAppSide _side;
    private readonly ILogger _vanillaLogger;

    /// <summary>
    ///     Initialises a new instance of the <see cref="GantryLogger"/> class.
    /// </summary>
    protected GantryLogger(EnumAppSide side, ILogger vanillaLogger) : base(
        program: $"Gantry {side}",
        clearOldFiles: true,
        archiveLogFileCount: 10,
        archiveLogFileMaxSize: 1024)
    {
        _side = side;
        _vanillaLogger = vanillaLogger;
    }

    /// <summary>
    ///     Initialises a new instance of the <see cref="GantryLogger"/> class.
    /// </summary>
    public static GantryLogger Create(EnumAppSide side, ILogger vanillaLogger, ModInfo modInfo)
    {
        ModInfo = modInfo;
        vanillaLogger.Debug($"[Gantry] Initialising Gantry {side} logger.");

        return side switch
        {
            EnumAppSide.Server => new ServerLogger(side, vanillaLogger),
            EnumAppSide.Client => new ClientLogger(side, vanillaLogger),
            _ => throw new UnreachableException()
        };
    }

    /// <summary>
    ///     The game-collated information for the current mod.
    /// </summary>
    protected static ModInfo ModInfo { get; private set; } = default!;

    /// <inheritdoc />
    public abstract override string getLogFile(EnumLogType logType);

    /// <summary>
    ///     Gets the file path for a specific log type.
    ///     
    ///     DEV-NOTE: Linux uses a case-sensitive file system, so we ensure the log file names are lowercased to avoid issues.
    /// </summary>
    /// <param name="logType">The type of log.</param>
    /// <param name="side">The app side of the log file.</param>
    /// <returns>The file path for the specified log type.</returns>
    protected string GetLogFilePath(EnumLogType logType, EnumAppSide side)
    {
        var directory = Directory.CreateDirectory(Path.Combine(GamePaths.Logs, "gantry", ModInfo.ModID));
        return Path.Combine(directory.FullName, $"{side}-{logType}.log".ToLowerInvariant());
    }

    /// <summary>
    ///     Logs a message with the specified log type, applying custom formatting.
    /// </summary>
    /// <param name="logFileName">The name of the file to write to.</param>
    /// <param name="logType">The type of log.</param>
    /// <param name="message">The message to log.</param>
    /// <param name="args">Optional arguments for message formatting.</param>
    public override void LogToFile(string logFileName, EnumLogType logType, string message, params object[] args)
    {
        if (!fileWriters.TryGetValue(logFileName, out var value) || disposed) return;

        try
        {
            var num = LinesWritten[logFileName];
            LinesWritten[logFileName] = num + 1U;

            if (LinesWritten[logFileName] > LogFileSplitAfterLine)
            {
                LinesWritten[logFileName] = 0U;

                var filename = $"{logFileName.Replace(".log", "")}-{LogfileSplitNumbers[logFileName]}.log";

                fileWriters[logFileName].Dispose();
                fileWriters[logFileName] = new DisposableWriter(filename, true);

                LogfileSplitNumbers[logFileName]++;
            }

            var type = logType == EnumLogType.StoryEvent ? "Event" : logType.ToString() ?? string.Empty;
            var timestampFormat = logType == EnumLogType.VerboseDebug ? "d.M.yyyy HH:mm:ss.fff" : "d.M.yyyy HH:mm:ss";
            var timestamp = DateTime.Now.ToString(timestampFormat);
            value.writer.WriteLine($"{timestamp} [{type}] [{_side}] [{ModInfo.Name}] {string.Format(message, args)}");
            value.writer.Flush();
        }
        catch (FormatException)
        {
            if (!exceptionPrinted)
            {
                exceptionPrinted = true;
                _vanillaLogger.Error("Couldn't write to Gantry log file, failed formatting {0} (FormatException)", message);
            }
        }
        catch (Exception e)
        {
            if (!exceptionPrinted)
            {
                exceptionPrinted = true;
                _vanillaLogger.Error("Couldn't write to Gantry log file {0}!", logFileName);
                _vanillaLogger.Error(e);
            }
        }
    }

    private class ServerLogger(EnumAppSide side, ILogger vanillaLogger)
        : GantryLogger(side, vanillaLogger)
    {
        public override string getLogFile(EnumLogType logType)
            => GetLogFilePath(logType, EnumAppSide.Server);

        public override bool printToConsole(EnumLogType logType)
            => logType is not EnumLogType.VerboseDebug
                      and not EnumLogType.StoryEvent
                      and not EnumLogType.Build
                      and not EnumLogType.Audit;

        public override bool printToDebugWindow(EnumLogType logType)
            => logType is not EnumLogType.VerboseDebug
                      and not EnumLogType.StoryEvent
                      and not EnumLogType.Build;
    }

    private class ClientLogger(EnumAppSide side, ILogger vanillaLogger)
        : GantryLogger(side, vanillaLogger)
    {
        public override string getLogFile(EnumLogType logType)
            => GetLogFilePath(logType, EnumAppSide.Client);

        public override bool printToConsole(EnumLogType logType)
            => logType is not EnumLogType.VerboseDebug
                      and not EnumLogType.StoryEvent;

        public override bool printToDebugWindow(EnumLogType logType)
            => logType is not EnumLogType.VerboseDebug
                      and not EnumLogType.StoryEvent;
    }
}