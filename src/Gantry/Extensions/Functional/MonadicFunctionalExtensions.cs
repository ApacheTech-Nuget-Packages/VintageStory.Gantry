
using ApacheTech.Common.FunctionalCSharp.Monads;

namespace Gantry.Extensions.Functional;

/// <summary>
///     Extension methods for monadic functions.
/// </summary>
internal static class MonadicFunctionalExtensions
{
    /// <summary>
    ///     Invokes an action on a monad of type <see cref="Identity{TFromType}"/>.
    /// </summary>
    /// <typeparam name="TFrom">The type of the value wrapped within the input monad.</typeparam>
    /// <param name="this">The monad to bind the function to.</param>
    /// <param name="f">The function to bind.</param>
    /// <returns>Returns the result of the bound operation, as an instance of <see cref="Identity{TTo}"/>.</returns>
    public static void Invoke<TFrom>(this Identity<TFrom> @this, Action<TFrom> f) => f(@this);
}
