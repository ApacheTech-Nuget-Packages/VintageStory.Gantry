#nullable enable

namespace Gantry.Core.Monads;

/// <summary>
///     Wraps the value in a monad, allowing functional style operations to be executed.
/// </summary>
/// <typeparam name="T">The type of value to wrap.</typeparam>
public class Sided<T>()
{
    private T _clientT = default!;
    private T _serverT = default!;

    /// <summary>
    ///     Gets the raw wrapped value.
    /// </summary>
    /// <value>
    ///     The value wrapped within this monad.
    /// </value>
    public T Current
    {
        get => ApiEx.OneOf(_clientT, _serverT);
        set => ApiEx.Run(() => _clientT = value, () => _serverT = value);
    }

    /// <summary>
    ///     Gets the raw wrapped value.
    /// </summary>
    /// <value>
    ///     The value wrapped within this monad.
    /// </value>
    public T Client => _clientT;

    /// <summary>
    ///     Gets the raw wrapped value.
    /// </summary>
    /// <value>
    ///     The value wrapped within this monad.
    /// </value>
    public T Server => _serverT;

    /// <summary>
    ///     Performs an implicit conversion from <typeparamref name="T"/> to <see cref="Sided{T}"/>.
    /// </summary>
    /// <param name="this">The instance of <typeparamref name="T"/> to convert.</param>
    /// <returns>
    ///     The result of the conversion.
    /// </returns>
    public static implicit operator Sided<T>(T @this) => @this.ToSided();

    /// <summary>
    ///     Performs an implicit conversion from <see cref="Sided{T}"/> to <typeparamref name="T"/>.
    /// </summary>
    /// <param name="this">The instance of <see cref="Sided{T}"/> to convert.</param>
    /// <returns>
    ///     The result of the conversion.
    /// </returns>
    public static implicit operator T(Sided<T> @this) => @this.Current;
}