namespace Gantry.Tools.Common.Extensions;

/// <summary>
///     Provides extension methods for string comparison and matching.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    ///     Determines whether the specified string matches any of the provided values, using a case-insensitive comparison.
    /// </summary>
    /// <param name="value">The string to compare.</param>
    /// <param name="values">The set of values to match against.</param>
    /// <returns><c>true</c> if <paramref name="value"/> matches any value in <paramref name="values"/>; otherwise, <c>false</c>.</returns>
    public static bool In(this string value, params string[] values) 
        => values.Any(v => v.Equals(value, StringComparison.OrdinalIgnoreCase));
}