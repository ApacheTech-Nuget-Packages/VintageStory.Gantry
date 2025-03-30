using System.Runtime.CompilerServices;

// ReSharper disable CommentTypo

namespace Gantry.Core.Diagnostics;

/// <summary>
///     Logs the execution time of a method via a disposable pattern.
/// </summary>
public sealed class LogExecutionTime : IDisposable
{
    private readonly Stopwatch _stopwatch;
    private readonly string _methodName;

    /// <summary>
    ///     Initialises a new instance of the <see cref="LogExecutionTime"/> class.
    /// </summary>
    /// <param name="methodName">The name of the method; automatically captured if not specified.</param>
    public LogExecutionTime([CallerMemberName] string methodName = "")
    {
        _methodName = methodName!;
        _stopwatch = Stopwatch.StartNew();
    }

    /// <summary>
    ///     Stops the timer and logs the execution time.
    /// </summary>
    public void Dispose()
    {
        _stopwatch.Stop();
        G.Logger.VerboseDebug($"{_methodName} executed in {_stopwatch.ElapsedMilliseconds} ms");
    }
}