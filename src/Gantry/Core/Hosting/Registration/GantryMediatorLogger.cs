using ApacheTech.Common.Mediator.Hosting;

namespace Gantry.Core.Hosting.Registration;

internal class GantryMediatorLogger : IMediatorLogger
{
    private readonly ILogger _logger;

    public GantryMediatorLogger(ILogger logger)
        => _logger = logger;

    public void LogCritical(string message)
        => _logger.Fatal(message);

    public void LogCritical(Exception ex, string message)
        => _logger.Fatal(ex);

    public void LogDebug(string message)
        => _logger.VerboseDebug(message);

    public void LogError(string message)
        => _logger.Error(message);

    public void LogError(Exception ex, string message)
        => _logger.Error(ex);

    public void LogInfo(string message)
        => _logger.VerboseDebug(message);

    public void LogTrace(string message)
        => _logger.VerboseDebug(message);

    public void LogWarning(string message)
        => _logger.Warning(message);

    public void LogWarning(Exception ex, string message)
        => _logger.Warning(ex);
}