using System.Drawing;

namespace Gantry.Core.Maths;

/// <summary>
///     Provides utilities for filtering and analysing colours based on their proximity in RGB space.
/// </summary>
public static class ColourExtension
{
    /// <summary>
    ///     Filters a list of colours by removing those too close to black, white, or other already-added colours.
    /// </summary>
    /// <param name="colours">The list of colours to filter.</param>
    /// <param name="threshold">The distance threshold used to determine whether a colour is too close to another.</param>
    /// <returns> A filtered list of colours containing distinct, sufficiently spaced colours.</returns>
    public static IEnumerable<Color> FilterSimilarColours(this IEnumerable<Color> colours, double threshold)
        => colours.Aggregate(new List<Color> { Color.Black, Color.White },
           (result, colour) => result.Any(c => colour.ColourDistance(c) < threshold) ? result : [.. result, colour]);

    /// <summary>
    ///     Determines whether a colour is too close to white, based on a specified distance threshold.
    /// </summary>
    /// <param name="colour">The source colour to evaluate.</param>
    /// <param name="target">The target colour to evaluate.</param>
    /// <param name="threshold">The distance threshold for proximity to white.</param>
    /// <returns><c>true</c> if the colour is too close to white; otherwise, <c>false</c>.</returns>
    public static bool IsSimilarTo(this Color colour, Color target, double threshold) 
        => colour.ColourDistance(target) < threshold;

    /// <summary>
    ///     Calculates the Euclidean distance between two colours in RGB space.
    /// </summary>
    /// <param name="c1">The first colour.</param>
    /// <param name="c2">The second colour.</param>
    /// <returns>The Euclidean distance between the two colours in RGB space.</returns>
    public static double ColourDistance(this Color c1, Color c2)
    {
        // Euclidean distance in RGB space
        var rDiff = c1.R - c2.R;
        var gDiff = c1.G - c2.G;
        var bDiff = c1.B - c2.B;

        return Math.Sqrt(rDiff * rDiff + gDiff * gDiff + bDiff * bDiff);
    }
}