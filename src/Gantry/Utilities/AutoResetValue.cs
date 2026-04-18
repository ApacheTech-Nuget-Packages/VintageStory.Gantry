using Timer = System.Threading.Timer;

namespace Gantry.Utilities;

/// <summary>
///     A value container that automatically resets to its initial value after a specified time-to-live has elapsed.
/// </summary>
public sealed class AutoResetValue<T> : IDisposable
{
    private readonly T? _initialValue;
    private readonly TimeSpan _ttl;
    private Timer? _timer;

    /// <summary>
    ///     Initialises a new instance of the <see cref="AutoResetValue{T}"/> class with the specified time-to-live and initial value.
    /// </summary>
    /// <param name="ttl">The time-to-live for the value before it resets.</param>
    /// <param name="initialValue">The initial value to be set.</param>
    public AutoResetValue(TimeSpan ttl, T? initialValue = default)
    {
        _initialValue = initialValue;
        _ttl = ttl;
        Value = initialValue;
    }

    /// <summary>
    ///     The current value, which resets to the initial value after the TTL has elapsed since the last call to <see cref="Set"/>.
    /// </summary>
    public T? Value { get; private set; }

    /// <summary>
    ///     Resets the value to the initial value and cancels any pending auto-reset timer.
    /// </summary>
    public void ManualReset()
    {
        _timer?.Dispose();
        _timer = null;
        Value = _initialValue;
    }

    /// <summary>
    ///     Assigns a new value and starts the auto-reset timer.
    /// </summary>
    public void Set(T value)
    {
        _timer?.Dispose();
        Value = value;
        _timer = new Timer(_ => Value = _initialValue, null, _ttl, Timeout.InfiniteTimeSpan);
    }

    /// <summary>
    ///     Disposes of the auto-reset timer, preventing any future automatic resets. 
    ///     After calling this method, the value will no longer reset automatically and will need to be manually reset if desired.
    /// </summary>
    public void Dispose()
    {
        _timer?.Dispose();
        _timer = null;
    }
}