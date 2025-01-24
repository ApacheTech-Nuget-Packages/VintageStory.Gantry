#nullable enable
namespace Gantry.Core.Monads;

/// <summary>
///     Extension methods for monadic functions.
/// </summary>
public static class SidedMonadicExtensions
{
    /// <summary>
    ///     Wraps the value in a monad, allowing functional style operations to be executed.
    /// </summary>
    /// <param name="value">The value to wrap.</param>
    /// <param name="instance">The wrapped instance.</param>
    /// <returns>
    ///     An instance of <see cref="Sided{T}"/>, which wraps the value, and provides access to functional style monadic operations.
    /// </returns>
    public static Sided<T> ToSided<T>(this T value, Sided<T>? instance = null)
    {
        instance ??= new Sided<T>();
        instance.Current = value;
        return instance;
    }
}