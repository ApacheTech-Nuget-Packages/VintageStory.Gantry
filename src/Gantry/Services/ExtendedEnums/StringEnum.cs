using System.Runtime.CompilerServices;

namespace Gantry.Services.ExtendedEnums;

/// <summary>
///     Simple implementation of a string-based enumeration, allowing
///     equatable string constants that can be implicitly cast to strings.
/// </summary>
/// <typeparam name="T"></typeparam>
/// <seealso cref="IEquatable{T}" />
public abstract class StringEnum<T> : ExtendedEnum<string, T> where T : StringEnum<T>, new()
{
    /// <summary>
    ///     Converts the string representation of the name or numeric value of one or more enumerated constants to an equivalent enumerated object.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="caseSensitive">If set to <c>true</c>, the value will be parsed as a case sensitive string. Default is False.</param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static T? Parse(string value, bool caseSensitive = false)
    {
        if (TryParse(value, caseSensitive, out var obj)) return obj;
        throw new InvalidOperationException(
            $"{(value == null ? "null" : $"'{value}'")} is not a valid {typeof(T).Name}");
    }

    /// <summary>
    ///     Converts the string representation of the name or numeric value of one or more enumerated constants to an equivalent enumerated object.
    ///     A parameter specifies whether the operation is case-sensitive. The return value indicates whether the conversion succeeded.
    /// </summary>
    /// <param name="value">The string representation of the enumeration name or underlying value to convert.</param>
    /// <param name="caseSensitive"><c>false</c> to ignore case; <c>true</c> to consider case.</param>
    /// <param name="result">
    ///     When this method returns, result contains an object of type T whose value is represented by value if the parse operation succeeds.
    ///     If the parse operation fails, result contains the default value of the underlying type of T.
    ///     Note that this value need not be a member of the T enumeration.
    ///     This parameter is passed uninitialised.
    /// </param>
    /// <returns>true if the <paramref name="value">value</paramref> parameter was converted successfully; otherwise, false.</returns>
    public static bool TryParse(string value, bool caseSensitive, out T? result)
    {
        result = null;
        if (value == null)
            return false;
        if (ValueDict.Count == 0)
            RuntimeHelpers.RunClassConstructor(typeof(T).TypeHandle);
        if (caseSensitive) return ValueDict.TryGetValue(value, out result);
        result = ValueDict.FirstOrDefault(f => f.Key.Equals(value, StringComparison.OrdinalIgnoreCase)).Value;
        return result is not null;
    }
}