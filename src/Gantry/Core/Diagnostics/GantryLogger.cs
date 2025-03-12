using Gantry.Services.FileSystem;
using Vintagestory;

namespace Gantry.Core.Diagnostics;

/// <summary>
///     A logger implementation for the Gantry mod, providing customised log handling and formatting.
/// </summary>
public abstract class GantryLogger : Logger
{
    private readonly string _modName;
    private readonly ICoreAPI _api;

    internal class Server(ICoreAPI api, ModInfo modInfo) : GantryLogger(api, modInfo)
    {
        public override string getLogFile(EnumLogType logType)
            => GetLogFilePath(logType, EnumAppSide.Server);
    }

    internal class Client(ICoreAPI api, ModInfo modInfo) : GantryLogger(api, modInfo)
    {
        public override string getLogFile(EnumLogType logType)
            => GetLogFilePath(logType, EnumAppSide.Client);
    }

    /// <inheritdoc />
    public abstract override string getLogFile(EnumLogType logType);

    /// <summary>
    ///     Gets the file path for a specific log type.
    ///     
    ///     LINUX: CASE SENSITIVE FILE SYSTEM
    /// </summary>
    /// <param name="logType">The type of log.</param>
    /// <param name="side">The app side of the log file.</param>
    /// <returns>The file path for the specified log type.</returns>
    protected string GetLogFilePath(EnumLogType logType, EnumAppSide side)
        => Path.Combine(ModPaths.LogDirectory.FullName, $"{side}-{logType}.log".ToLowerInvariant());

    /// <summary>
    ///     Initialises a new instance of the <see cref="GantryLogger"/> class.
    /// </summary>
    public GantryLogger(ICoreAPI api, ModInfo modInfo) : base(
        program: $"Gantry {api.Side}",
        clearOldFiles: true,
        archiveLogFileCount: 10,
        archiveLogFileMaxSize: 1024)
    {
        _modName = modInfo.Name;
        _api = api;
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
            value.writer.WriteLine($"{timestamp} [{type}] [{_modName}] {message}", args);
            value.writer.Flush();
        }
        catch (FormatException)
        {
            if (!exceptionPrinted)
            {
                exceptionPrinted = true;
                _api.Logger.Error("Couldn't write to Gantry log file, failed formatting {0} (FormatException)", message);
            }
        }
        catch (Exception e)
        {
            if (!exceptionPrinted)
            {
                exceptionPrinted = true;
                _api.Logger.Error("Couldn't write to Gantry log file {0}!", logFileName);
                _api.Logger.Error(e);
            }
        }
    }

    /// <summary>
    ///     Determines whether the specified log type should be printed to the console.
    /// </summary>
    /// <param name="logType">The type of log.</param>
    /// <returns><c>true</c> if the log type should be printed to the console; otherwise, <c>false</c>.</returns>
    public override bool printToConsole(EnumLogType logType)
        => logType is not EnumLogType.VerboseDebug
           and not EnumLogType.StoryEvent
           and not EnumLogType.Audit;
}