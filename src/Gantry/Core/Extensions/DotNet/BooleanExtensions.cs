namespace Gantry.Core.Extensions.DotNet;

/// <summary>
///     Provides extension methods to enhance the functionality of boolean values, enabling fluent conditional logic and action invocation.
/// </summary>
public static class BooleanExtensions
{
    /// <summary>
    ///     Invokes an action if the condition is true.
    /// </summary>
    /// <param name="condition">The boolean condition to evaluate.</param>
    /// <param name="trueAction">The action to invoke if <paramref name="condition"/> is true.</param>
    public static void If(this bool condition, Action trueAction)
    {
        if (condition) trueAction();
    }

    /// <summary>
    ///     Invokes an action if the condition is false.
    /// </summary>
    /// <param name="condition">The boolean condition to evaluate.</param>
    /// <param name="falseAction">The action to invoke if <paramref name="condition"/> is false.</param>
    public static void IfNot(this bool condition, Action falseAction)
    {
        if (!condition) falseAction();
    }

    /// <summary>
    ///     Invokes one of two actions, depending on whether the condition is true or false.
    /// </summary>
    /// <param name="condition">The boolean condition to evaluate.</param>
    /// <param name="trueAction">The action to invoke if <paramref name="condition"/> is true.</param>
    /// <param name="falseAction">The action to invoke if <paramref name="condition"/> is false.</param>
    public static void IfElse(this bool condition, Action trueAction, Action falseAction)
    {
        if (condition) trueAction();
        else falseAction();
    }

    /// <summary>
    ///     Invokes an action with an argument if the condition is true.
    /// </summary>
    /// <typeparam name="T">The type of the argument to pass to the action.</typeparam>
    /// <param name="condition">The boolean condition to evaluate.</param>
    /// <param name="trueAction">The action to invoke if <paramref name="condition"/> is true.</param>
    /// <param name="args">The argument to pass to the action.</param>
    public static void If<T>(this bool condition, Action<T> trueAction, T args)
    {
        if (condition) trueAction(args);
    }

    /// <summary>
    ///     Invokes an action with an argument if the condition is false.
    /// </summary>
    /// <typeparam name="T">The type of the argument to pass to the action.</typeparam>
    /// <param name="condition">The boolean condition to evaluate.</param>
    /// <param name="falseAction">The action to invoke if <paramref name="condition"/> is false.</param>
    /// <param name="args">The argument to pass to the action.</param>
    public static void IfNot<T>(this bool condition, Action<T> falseAction, T args)
    {
        if (!condition) falseAction(args);
    }

    /// <summary>
    ///     Invokes one of two actions with an argument, depending on whether the condition is true or false.
    /// </summary>
    /// <typeparam name="T">The type of the argument to pass to the actions.</typeparam>
    /// <param name="condition">The boolean condition to evaluate.</param>
    /// <param name="trueAction">The action to invoke if <paramref name="condition"/> is true.</param>
    /// <param name="falseAction">The action to invoke if <paramref name="condition"/> is false.</param>
    /// <param name="args">The argument to pass to the actions.</param>
    public static void IfElse<T>(this bool condition, Action<T> trueAction, Action<T> falseAction, T args)
    {
        if (condition) trueAction(args);
        else falseAction(args);
    }

    /// <summary>
    ///     Invokes one of two functions and returns the result, depending on whether the condition is true or false.
    /// </summary>
    /// <typeparam name="T">The return type of the functions.</typeparam>
    /// <param name="condition">The boolean condition to evaluate.</param>
    /// <param name="trueFunction">The function to invoke if <paramref name="condition"/> is true.</param>
    /// <param name="falseFunction">The function to invoke if <paramref name="condition"/> is false.</param>
    /// <returns>The result of the invoked function.</returns>
    public static T IfElse<T>(this bool condition, Func<T> trueFunction, Func<T> falseFunction)
    {
        return condition ? trueFunction() : falseFunction();
    }

    /// <summary>
    ///     Invokes one of two functions with an argument and returns the result, depending on whether the condition is true or false.
    /// </summary>
    /// <typeparam name="TIn">The type of the argument to pass to the functions.</typeparam>
    /// <typeparam name="TOut">The return type of the functions.</typeparam>
    /// <param name="condition">The boolean condition to evaluate.</param>
    /// <param name="trueFunction">The function to invoke if <paramref name="condition"/> is true.</param>
    /// <param name="falseFunction">The function to invoke if <paramref name="condition"/> is false.</param>
    /// <param name="args">The argument to pass to the functions.</param>
    /// <returns>The result of the invoked function.</returns>
    public static TOut IfElse<TIn, TOut>(this bool condition, System.Func<TIn, TOut> trueFunction, System.Func<TIn, TOut> falseFunction, TIn args)
    {
        return condition ? trueFunction(args) : falseFunction(args);
    }

    /// <summary>
    ///     Performs a <see cref="Action"/> if the input boolean value is true.
    /// </summary>
    /// <param name="state">The boolean value that this extension method was called on.</param>
    /// <param name="trueAction">The action to perform, if <paramref name="state"/> equates to <c>true</c>.</param>
    /// <returns>The same boolean value as was passed into the method, as <paramref name="state"/></returns>
    public static bool ActIfTrue(this bool state, Action trueAction)
    {
        if (state) trueAction();
        return state;
    }

    /// <summary>
    ///     Performs a <see cref="Action{T}"/> if the input boolean value is true.
    /// </summary>
    /// <typeparam name="T">The type to pass into the action.</typeparam>
    /// <param name="state">The boolean value that this extension method was called on.</param>
    /// <param name="trueAction">The action to perform, if <paramref name="state"/> equates to <c>true</c>.</param>
    /// <param name="args">The arguments to pass to the action.</param>
    /// <returns>The same boolean value as was passed into the method, as <paramref name="state"/></returns>
    public static bool ActIfTrue<T>(this bool state, Action<T> trueAction, T args)
    {
        if (state) trueAction(args);
        return state;
    }

    /// <summary>
    ///     Performs a <see cref="Action"/> if the input boolean value is false.
    /// </summary>
    /// <param name="state">The boolean value that this extension method was called on.</param>
    /// <param name="falseAction">The action to perform, if <paramref name="state"/> equates to <c>false</c>.</param>
    /// <returns>The same boolean value as was passed into the method, as <paramref name="state"/></returns>
    public static bool ActIfFalse(this bool state, Action falseAction)
    {
        if (!state) falseAction();
        return state;
    }

    /// <summary>
    ///     Performs a <see cref="Action{T}"/> if the input boolean value is false.
    /// </summary>
    /// <typeparam name="T">The type to pass into the action.</typeparam>
    /// <param name="state">The boolean value that this extension method was called on.</param>
    /// <param name="falseAction">The action to perform, if <paramref name="state"/> equates to <c>false</c>.</param>
    /// <param name="args">The arguments to pass to the action.</param>
    /// <returns>The same boolean value as was passed into the method, as <paramref name="state"/></returns>
    public static bool ActIfFalse<T>(this bool state, Action<T> falseAction, T args)
    {
        if (!state) falseAction(args);
        return state;
    }

    /// <summary>
    ///     Performs a <see cref="Action"/> if the input boolean value is true or false.
    /// </summary>
    /// <param name="state">The boolean value that this extension method was called on.</param>
    /// <param name="trueAction">The action to perform, if <paramref name="state"/> equates to <c>true</c>.</param>
    /// <param name="falseAction">The action to perform, if <paramref name="state"/> equates to <c>false</c>.</param>
    /// <returns>The same boolean value as was passed into the method, as <paramref name="state"/></returns>
    public static bool ActIf(this bool state, Action trueAction, Action falseAction)
    {
        if (state) trueAction();
        else falseAction();
        return state;
    }

    /// <summary>
    ///     Performs a <see cref="Action{T}"/> if the input boolean value is true.
    /// </summary>
    /// <typeparam name="T">The type to pass into the action.</typeparam>
    /// <param name="state">The boolean value that this extension method was called on.</param>
    /// <param name="trueAction">The action to perform, if <paramref name="state"/> equates to <c>true</c>.</param>
    /// <param name="falseAction">The action to perform, if <paramref name="state"/> equates to <c>false</c>.</param>
    /// <param name="args">The arguments to pass to the action.</param>
    /// <returns>The same boolean value as was passed into the method, as <paramref name="state"/></returns>
    public static bool ActIf<T>(this bool state, Action<T> trueAction, Action<T> falseAction, T args)
    {
        if (state) trueAction(args);
        else falseAction(args);
        return state;
    }
}