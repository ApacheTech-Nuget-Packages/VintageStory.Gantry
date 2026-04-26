namespace Gantry.Extensions;

/// <summary>
///     Provides extension methods for validating string values.
/// </summary>
public static class StringGuardExtensions
{
    /// <summary>
    ///     Throws an <see cref="ArgumentNullException"/> if the specified string is <c>null</c>.
    /// </summary>
    /// <param name="value">The string to validate.</param>
    /// <param name="nameOf">The name of the parameter being validated.</param>
    /// <param name="message">The exception message to include if the value is <c>null</c>.</param>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when <paramref name="value"/> is <c>null</c>.
    /// </exception>
    public static void ThrowIfNull(this string value, string nameOf, string message)
    {
        if (value is null)
        {
            throw new ArgumentNullException(nameOf, message);
        }
    }

    /// <summary>
    ///     Throws an <see cref="ArgumentException"/> if the specified string is <c>null</c> or empty.
    /// </summary>
    /// <param name="value">The string to validate.</param>
    /// <param name="nameOf">The name of the parameter being validated.</param>
    /// <param name="message">The exception message to include if the value is invalid.</param>
    /// <exception cref="ArgumentException">
    ///     Thrown when <paramref name="value"/> is <c>null</c> or an empty string.
    /// </exception>
    public static void ThrowIfNullOrEmpty(this string value, string nameOf, string message)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new ArgumentException(message, nameOf);
        }
    }

    /// <summary>
    ///     Throws an <see cref="ArgumentException"/> if the specified string is <c>null</c>, empty, or consists only of whitespace.
    /// </summary>
    /// <param name="value">The string to validate.</param>
    /// <param name="nameOf">The name of the parameter being validated.</param>
    /// <param name="message">The exception message to include if the value is invalid.</param>
    /// <exception cref="ArgumentException">
    ///     Thrown when <paramref name="value"/> is <c>null</c>, empty, or whitespace.
    /// </exception>
    public static void ThrowIfNullOrWhitespace(this string value, string nameOf, string message)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException(message, nameOf);
        }
    }

    /// <summary>
    ///     Returns <c>true</c> if the specified string is equal to any of the provided options, ignoring case and leading/trailing whitespace; otherwise, returns <c>false</c>.
    /// </summary>
    /// <param name="value">The string to validate.</param>
    /// <param name="options">The options to compare against.</param>
    /// <returns><c>true</c> if the string matches any option; otherwise, <c>false</c>.</returns>
    public static bool In(this string? value, params string[] options)
    {
        if (string.IsNullOrEmpty(value)) return false;
        return options.Contains(value.Trim(), StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    ///     Determines whether the specified string represents a logical 'true' value, using common affirmative terms and
    ///     any additional special cases provided.
    /// </summary>
    /// <param name="value">The string to evaluate for a truthy value. Can be null.</param>
    /// <param name="specialCases">An optional array of additional string values that should be considered as representing 'true'. Comparison is case-insensitive.</param>
    /// <returns>true if the string matches a recognised affirmative value or any of the specified special cases; otherwise,
    /// false.</returns>
    public static bool IsTruthy(this string? value, params string[] specialCases)
    {
        if (string.IsNullOrEmpty(value)) return false;
        return value.In(["true", "yes", "y", "t", "1", "on", "enabled", "enable", "ok", "okay", "affirmative", "yep", "yeah", "si", "oui", .. specialCases]);
    }

    /// <summary>
    ///     Determines whether the specified string is considered 'falsey', matching common representations of false values.
    /// </summary>
    /// <param name="value">The string to evaluate for a 'falsey' value. May be null.</param>
    /// <param name="specialCases">Additional string values to treat as 'falsey' in addition to the built-in set. Comparison is case-insensitive.</param>
    /// <returns>true if the string is null, empty, or matches any of the recognised 'falsey' values; otherwise, false.</returns>
    public static bool IsFalsey(this string? value, params string[] specialCases)
    {
        if (!string.IsNullOrEmpty(value)) return true;
        return value.In(["false", "no", "n", "f", "0", "off", "disabled", "disable", "nok", "negative", "nope", "non", "nicht", .. specialCases]);
    }
}