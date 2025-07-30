namespace Gantry.Extensions.DotNet;

/// <summary>
///    Extension methods to aid with functional programming in C#.
/// </summary>
public static class FunctionalExtensions
{
    /// <summary>
    ///     Performs an operation repeatedly, until the criteria for stopping has been met.
    /// </summary>
    /// <typeparam name="T">The type of operation handler.</typeparam>
    /// <param name="this">The operation function to run.</param>
    /// <param name="createNext">A function that changes the state of the operation, between iterations.</param>
    /// <param name="finishCondition">A predicate that determines whether the criteria for stopping the iteration has been met.</param>
    /// <returns></returns>
    public static T IterateUntil<T>(this T @this, System.Func<T, T> createNext, System.Func<T, bool> finishCondition) =>
        finishCondition(@this) ? @this : createNext(@this).IterateUntil(createNext, finishCondition);

    /// <summary>
    ///     Invokes the specified action if all conditions are met.
    /// </summary>
    /// <typeparam name="T">The type of the object.</typeparam>
    /// <param name="this">The object to evaluate.</param>
    /// <param name="action">The action to invoke if all conditions are met.</param>
    /// <param name="tests">The conditions that must all be met for the action to be invoked.</param>
    /// <remarks>
    ///     If the object is <c>null</c>, the method returns without invoking the action.
    ///     If all provided conditions evaluate to <c>true</c>, the action is executed.
    /// </remarks>
    public static void InvokeIf<T>(this T @this, Action<T> action, params System.Func<T, bool>[] tests)
    {
        if (@this is null) return;
        if (tests.All(f => f(@this))) action(@this);
    }

    /// <summary>
    ///     Invokes the specified action unless any condition is met.
    /// </summary>
    /// <typeparam name="T">The type of the object.</typeparam>
    /// <param name="this">The object to evaluate.</param>
    /// <param name="action">The action to invoke if none of the conditions are met.</param>
    /// <param name="tests">The conditions that prevent the action from being invoked.</param>
    /// <remarks>
    ///     If the object is <c>null</c>, the method returns without invoking the action.
    ///     If any provided condition evaluates to <c>true</c>, the action is not executed.
    /// </remarks>
    public static void InvokeUnless<T>(this T @this, Action<T> action, params System.Func<T, bool>[] tests)
    {
        if (@this is null) return;
        if (tests.Any(f => f(@this))) return;
        action(@this);
    }

}