namespace Gantry.Core.Extensions.Helpers;

/// <summary>
///     Helper class that extends the normal functionality of enums.
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class EnumEx
{
    /// <summary>
    ///     Converts the string representation of the name or numeric value of one or more enumerated constants to an
    ///     equivalent enumerated object. A parameter specifies whether the operation is case-insensitive.
    /// </summary>
    /// <typeparam name="TEnum">An enumeration type.</typeparam>
    /// <param name="value">A string containing the name or value to convert.</param>
    /// <param name="ignoreCase">true to ignore case; false to regard case.</param>
    /// <returns>
    ///     An object of type <typeparamref name="TEnum">enumType</typeparamref> whose value is represented by
    ///     <paramref name="value">value</paramref>.
    /// </returns>
    /// <exception cref="T:System.ArgumentNullException">
    ///     <typeparamref name="TEnum">enumType</typeparamref> or
    ///     <paramref name="value">value</paramref> is null.
    /// </exception>
    /// <exception cref="T:System.ArgumentException">
    ///     <typeparamref name="TEnum">enumType</typeparamref> is not an
    ///     <see cref="T:System.Enum"></see>.   -or-  <paramref name="value">value</paramref> is either an empty string ("") or
    ///     only contains white space.   -or-  <paramref name="value">value</paramref> is a name, but not one of the named
    ///     constants defined for the enumeration.
    /// </exception>
    /// <exception cref="T:System.OverflowException">
    ///     <paramref name="value">value</paramref> is outside the range of the
    ///     underlying type of <typeparamref name="TEnum">enumType</typeparamref>.
    /// </exception>
    public static TEnum Parse<TEnum>(string value, bool ignoreCase = false) where TEnum : Enum
    {
        return (TEnum)Enum.Parse(typeof(TEnum), value, ignoreCase);
    }

    /// <summary>
    ///     Invokes an action corresponding to the enumeration value, similar to a switch expression.
    /// </summary>
    /// <typeparam name="TEnum">The type of the enumeration. Must be an enumeration type.</typeparam>
    /// <param name="value">The enumeration value for which an action should be invoked.</param>
    /// <param name="cases">A parameter array of tuples, where each tuple consists of an enumeration case and the associated action to invoke.</param>
    /// <remarks>
    ///     The method iterates over the provided cases and compares the current enumeration value with each case.
    ///     If a match is found, the corresponding action is invoked and the method returns immediately.
    ///     If no matching case is found, no action is performed.
    /// </remarks>
    public static void Switch<TEnum>(this TEnum value, params (TEnum Case, Action Action)[] cases)
        where TEnum : Enum
    {
        foreach (var (enumCase, action) in cases)
        {
            if (EqualityComparer<TEnum>.Default.Equals(value, enumCase))
            {
                action?.Invoke();
                return;
            }
        }
    }
}