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
}