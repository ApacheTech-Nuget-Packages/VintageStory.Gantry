using Gantry.Extensions.Api;

namespace Gantry.Core.Helpers;

/// <summary>
///     A monad that holds a value of type <typeparamref name="T"/> for both the client and server sides.
/// </summary>
/// <typeparam name="T"></typeparam>
public class Sided<T>()
{
    private readonly AsyncLocal<T> _clientT = new();
    private readonly AsyncLocal<T> _serverT = new();
    private readonly AsyncLocal<Sided<T>> _self = new();

    private Sided(AsyncLocal<Sided<T>> asyncLocal) : this()
    {
        _self = asyncLocal;
        _self.Value = this;
    }

    /// <summary>
    ///     Creates a new instance of the <see cref="Sided{T}"/> class.
    /// </summary>
    /// <returns>A new instance of the <see cref="Sided{T}"/> class.</returns>
    public static Sided<T> AsyncLocal()
    {
        var sided = new Sided<T>(new());
        return sided._self.Value!;
    }

    /// <summary>
    ///     The value of the type <typeparamref name="T"/> for the client.
    /// </summary>
    public T Client
        => _clientT.Value 
        ?? throw new InvalidOperationException($"No value of type {typeof(T).FullName} has been set for the client side.");

    /// <summary>
    ///     The value of the type <typeparamref name="T"/> for the server.
    /// </summary>
    public T Server
        => _serverT.Value 
        ?? throw new InvalidOperationException($"No value of type {typeof(T).FullName} has been set for the server side.");

    /// <summary>
    ///     The value of the type <typeparamref name="T"/> for the current side (client or server).
    /// </summary>
    public T Current
        => _clientT.Value ?? _serverT.Value!;

    /// <summary>
    ///     Determines if a value of type <typeparamref name="T"/> has been set for either the client or server side.
    /// </summary>
    public bool HasValue(EnumAppSide side) => side.Invoke(
        () => _clientT.Value is not null, 
        () => _serverT.Value is not null);

    /// <summary>
    ///     Sets the value of type <typeparamref name="T"/> for the current side (client or server).
    /// </summary>
    /// <param name="side">The side for which to set the value.</param>
    /// <param name="value">The value to set.</param>
    public void Set(EnumAppSide side, T value)
    {
        side.Invoke(() =>
        {
            _clientT.Value ??= value;
        }, () =>
        {
            _serverT.Value ??= value;
        });
    }

    /// <summary>
    ///     Disposes the current instance, clearing the values for the current side (client or server).
    /// </summary>
    /// <param name="side">The side to dispose.</param>
    public void Dispose(EnumAppSide side)
    {
        side.Invoke(() =>
            {
                _clientT.Value ??= default!;
            }, () =>
            {
                _serverT.Value ??= default!;
            });

        if (_clientT.Value is null && _serverT.Value is null)
            _self.Value = null!;
    }
}