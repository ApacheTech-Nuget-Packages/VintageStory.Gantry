namespace Gantry.Core.Extensions.Api;

/// <summary>
///     Extension methods for logging, within Gantry.
/// </summary>
public static class LoggerExtensions
{
    /// <summary>
    ///     Adds a new <see cref="F:Vintagestory.API.Common.EnumLogType.Audit" /> log entry with the specified message, if debug mode is enabled.
    /// </summary>
    public static void Trace(this ILogger logger, string format, params object[] args)
    {
        logger.VerboseDebug(format, args);
    }
}