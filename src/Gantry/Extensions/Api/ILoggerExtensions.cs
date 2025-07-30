namespace Gantry.Extensions.Api;

/// <summary>
///     Provides extension methods for the <see cref="ILogger"/> interface, enhancing logging capabilities with additional features.
/// </summary>
public static class ILoggerExtensions
{
    /// <summary>
    ///     Highlights a message in the log, making it stand out for important information.
    /// </summary>
    /// <param name="logger">The logger instance to use for logging the highlighted message.</param>
    /// <param name="messageTemplate">The message template to log.</param>
    /// <param name="args">Arguments for formatting the message.</param>
    public static void Highlight(this ILogger logger, string messageTemplate, params object[] args)
    {
        logger.VerboseDebug("");
        logger.VerboseDebug("================================================================================");
        logger.VerboseDebug(messageTemplate, args);
        logger.VerboseDebug("================================================================================");
        logger.VerboseDebug("");
    }
}
