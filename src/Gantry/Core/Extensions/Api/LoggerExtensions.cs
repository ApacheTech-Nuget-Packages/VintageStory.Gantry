namespace Gantry.Core.Extensions.Api;

/// <summary>
///     Provides extension methods for logging within Gantry.
/// </summary>
public static class LoggerExtensions
{
    /// <summary>
    ///     Adds a new audit log entry with the specified message, if debug mode is enabled.
    ///     This method is an alias for <see cref="ILogger.VerboseDebug(string, object[])"/> and is intended for tracing execution flow or debugging information.
    /// </summary>
    /// <param name="logger">The logger instance to write the message to.</param>
    /// <param name="format">A composite format string (see <see cref="string.Format(string, object[])"/>) for the log message.</param>
    /// <param name="args">An array of objects to format.</param>
    public static void Trace(this ILogger logger, string format, params object[] args)
    {
        logger.VerboseDebug(format, args);
    }
}