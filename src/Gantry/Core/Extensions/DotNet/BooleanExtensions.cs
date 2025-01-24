namespace Gantry.Core.Extensions.DotNet;

/// <summary>
///     Extends the functionality of boolean values.
/// </summary>
public static class BooleanExtensions
{
    /// <summary>
    ///     Invokes an action, if the condition is true.
    /// </summary>
    /// <param name="condition">The condition.</param>
    /// <param name="trueAction">The action to invoke.</param>
    public static void If(this bool condition, Action trueAction)
    {
        if (condition) trueAction();
    }

    /// <summary>
    ///     Invokes an action, if the condition is false.
    /// </summary>
    /// <param name="condition">The condition.</param>
    /// <param name="falseAction">The action to invoke.</param>
    public static void IfNot(this bool condition, Action falseAction)
    {
        if (!condition) falseAction();
    }

    /// <summary>
    ///     Invokes an action, based on whether the condition is true, or false.
    /// </summary>
    /// <param name="condition">The condition.</param>
    /// <param name="trueAction">The action to invoke, if the condition is true.</param>
    /// <param name="falseAction">The action to invoke, if the condition is false.</param>
    public static void IfElse(this bool condition, Action trueAction, Action falseAction)
    {
        if (condition) trueAction();
        else falseAction();
    }

    /// <summary>
    ///     Invokes an action, if the condition is true.
    /// </summary>
    /// <param name="condition">The condition.</param>
    /// <param name="trueAction">The action to invoke.</param>
    /// <param name="args">The arguments to pass to the action.</param>
    public static void If<T>(this bool condition, Action<T> trueAction, T args)
    {
        if (condition) trueAction(args);
    }

    /// <summary>
    ///     Invokes an action, if the condition is false.
    /// </summary>
    /// <param name="condition">The condition.</param>
    /// <param name="falseAction">The action to invoke.</param>
    /// <param name="args">The arguments to pass to the action.</param>
    public static void IfNot<T>(this bool condition, Action<T> falseAction, T args)
    {
        if (!condition) falseAction(args);
    }

    /// <summary>
    ///     Invokes an action, based on whether the condition is true, or false.
    /// </summary>
    /// <param name="condition">The condition.</param>
    /// <param name="trueAction">The action to invoke, if the condition is true.</param>
    /// <param name="falseAction">The action to invoke, if the condition is false.</param>
    /// <param name="args">The arguments to pass to the action.</param>
    public static void IfElse<T>(this bool condition, Action<T> trueAction, Action<T> falseAction, T args)
    {
        if (condition) trueAction(args);
        else falseAction(args);
    }

    /// <summary>
    ///     Invokes a function, based on whether the condition is true, or false.
    /// </summary>
    /// <param name="condition">The condition.</param>
    /// <param name="trueFunction">The function to invoke, if the condition is true.</param>
    /// <param name="falseFunction">The function to invoke, if the condition is false.</param>
    public static T IfElse<T>(this bool condition, Func<T> trueFunction, Func<T> falseFunction)
    {
        return condition ? trueFunction() : falseFunction();
    }

    /// <summary>
    ///     Invokes a function, based on whether the condition is true, or false.
    /// </summary>
    /// <param name="condition">The condition.</param>
    /// <param name="trueFunction">The function to invoke, if the condition is true.</param>
    /// <param name="falseFunction">The function to invoke, if the condition is false.</param>
    /// <param name="args">The arguments to pass to the action.</param>
    public static TOut IfElse<TIn, TOut>(this bool condition, System.Func<TIn, TOut> trueFunction, System.Func<TIn, TOut> falseFunction, TIn args)
    {
        return condition ? trueFunction(args) : falseFunction(args);
    }
}