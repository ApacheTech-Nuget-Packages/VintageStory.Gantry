namespace Gantry.Core.Maths.Extensions;

/// <summary>
///     Provides extension methods for mathematical operations.S
/// </summary>
public static class MathsExtensions
{
    /// <summary>
    ///     Clamps a value to be outside the specified range.
    /// </summary>
    /// <typeparam name="T">The type of the value, which must implement <see cref="IComparable{T}"/>.</typeparam>
    /// <param name="value">The value to be clamped.</param>
    /// <param name="minValue">The minimum value of the range.</param>
    /// <param name="maxValue">The maximum value of the range.</param>
    /// <returns>
    ///     The original value if it lies outside the range; otherwise, the nearest boundary of the range 
    ///     that is farther away from the value.
    /// </returns>
    /// <remarks>
    ///     This method ensures that the returned value is not within the specified range. If the value 
    ///     lies within the range, it will be adjusted to either the minimum or maximum boundary, depending 
    ///     on which is farther from the value.
    /// </remarks>
    public static T InverseClamp<T>(this T value, T minValue, T maxValue) where T : IComparable<T>
    {
        if (value.CompareTo(minValue) < 0 || value.CompareTo(maxValue) > 0) return value;
        return value.CompareTo(minValue) - value.CompareTo(maxValue) > 0 ? maxValue : minValue;
    }

}