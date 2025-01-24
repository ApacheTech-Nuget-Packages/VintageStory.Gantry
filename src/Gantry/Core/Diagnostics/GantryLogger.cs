using Vintagestory;

namespace Gantry.Core.Diagnostics;

/// <summary>
///     A logger implementation for the Gantry mod, providing customised log handling and formatting.
/// </summary>
public class GantryLogger : Logger
{
    private static ICoreAPI _api;
    private static string _modName;

    /// <summary>
    ///     Creates and initialises a new instance of the <see cref="GantryLogger"/> class.
    /// </summary>
    /// <param name="api">Common API Components that are available on the server and the client.</param>
    /// <param name="modInfo">The unique information about the mod.</param>
    /// <returns>A new instance of the <see cref="GantryLogger"/>.</returns>
    internal static GantryLogger Create(ICoreAPI api, ModInfo modInfo)
    {
        _api = api;
        _modName = modInfo.Name;
        LogDirectory = new DirectoryInfo(Path.Combine(GamePaths.Logs, "gantry", modInfo.ModID));
        if (!LogDirectory.Exists) LogDirectory.Create();
        var logger = new GantryLogger();
        return logger;
    }

    /// <summary>
    ///     The directory that the log files are stored in.
    /// </summary>
    public static DirectoryInfo LogDirectory { get; private set; }

    /// <summary>
    ///     Initialises a new instance of the <see cref="GantryLogger"/> class.
    /// </summary>
    private GantryLogger() : base(program: $"Gantry {_api.Side}",
                                  clearOldFiles: true,
                                  archiveLogFileCount: 10,
                                  archiveLogFileMaxSize: 1024)
    {
    }

    /// <summary>
    ///     Gets the file path for a specific log type.
    /// </summary>
    /// <param name="logType">The type of log.</param>
    /// <returns>The file path for the specified log type.</returns>
    public override string getLogFile(EnumLogType logType)
    {
        var path = Path.Combine(LogDirectory.FullName, $"{_api.Side}-{logType}.txt");
        return path.ToLowerInvariant();
    }

    /// <summary>
    ///     Logs a message with the specified log type, applying custom formatting.
    /// </summary>
    /// <param name="logType">The type of log.</param>
    /// <param name="message">The message to log.</param>
    /// <param name="args">Optional arguments for message formatting.</param>
    protected override void LogImpl(EnumLogType logType, string message, params object[] args)
    {
        if (disposed) return;
        message = $"[{_modName}] {message}";
        base.LogImpl(logType, message, args);
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