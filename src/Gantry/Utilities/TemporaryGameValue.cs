namespace Gantry.Utilities;

/// <summary>
///     Represents a value that exists for a limited duration measured in game hours.
///     The value automatically clears itself once the duration has elapsed.
/// </summary>
/// <typeparam name="T">The type of value being stored.</typeparam>
public class TemporaryGameValue<T> : IDisposable
{
    private readonly ICoreGantryAPI _gantry;

    private long _listenerId;
    private bool _listenerRegistered;

    private double _resetTick;
    private bool _disposed;

    /// <summary>
    ///     Initialises a new instance bound to the provided gantry API.
    /// </summary>
    /// <param name="gantry">The gantry API used for time and event access.</param>
    public TemporaryGameValue(ICoreGantryAPI gantry)
    {
        _gantry = gantry;
    }

    /// <summary>
    ///     The current value. Will be reset to default once the timer expires.
    /// </summary>
    public T? Value { get; private set; } = default;

    /// <summary>
    ///     Assigns a value and starts (or restarts) the expiry timer.
    /// </summary>
    /// <param name="value">The value to store.</param>
    /// <param name="gameHours">The duration in game hours before the value is cleared.</param>
    public void SetValue(T value, double gameHours)
    {
        ThrowIfDisposed();

        Value = value;

        var now = _gantry.Uapi.World.Calendar.TotalHours;
        _resetTick = now + gameHours;

        EnsureListener();
    }

    /// <summary>
    ///     Extends the lifetime of the current value.
    /// </summary>
    /// <param name="gameHours">The new duration from the current time.</param>
    /// <remarks>
    ///     If the value has already expired, this will restart the timer but does not assign a new value.
    /// </remarks>
    public void Renew(double gameHours)
    {
        ThrowIfDisposed();

        var now = _gantry.Uapi.World.Calendar.TotalHours;
        _resetTick = now + gameHours;

        EnsureListener();
    }

    /// <summary>
    ///     Clears the value and stops the expiry listener.
    /// </summary>
    public void ClearValue()
    {
        Value = default;
        UnregisterListener();
    }

    /// <summary>
    ///     Ensures a single tick listener is registered.
    /// </summary>
    private void EnsureListener()
    {
        if (_listenerRegistered) return;

        _listenerId = _gantry.Uapi.Event.RegisterGameTickListener(dt =>
        {
            var now = _gantry.Uapi.World.Calendar.TotalHours;
            if (now >= _resetTick) ClearValue();
        }, 200);

        _listenerRegistered = true;
    }

    /// <summary>
    ///     Safely unregisters the tick listener if it exists.
    /// </summary>
    private void UnregisterListener()
    {
        if (!_listenerRegistered) return;

        _gantry.Uapi.Event.UnregisterGameTickListener(_listenerId);
        _listenerRegistered = false;
    }

    /// <summary>
    ///     Throws if the instance has been disposed.
    /// </summary>
    private void ThrowIfDisposed()
    {
        if (!_disposed) return;
        throw new ObjectDisposedException(nameof(TemporaryGameValue<>));
    }

    /// <summary>
    ///     Releases resources and unregisters any active listeners.
    /// </summary>
    public void Dispose()
    {
        if (_disposed) return;

        UnregisterListener();
        _disposed = true;

        GC.SuppressFinalize(this);
    }
}
