using System.Drawing;

namespace Gantry.Core.Maths;

/// <summary>
///     Provides a custom comparison for colours, using their hue, lightness, and brightness
///     to determine the sorting order.
/// </summary>
public class ColourRampComparer : IComparer<Color>
{
    /// <summary>
    ///     The number of repetitions used to quantise the step values.
    /// </summary>
    public int Repetitions { get; init; } = 8;

    /// <summary>
    ///     Mirrors the brightness scale on either side of each repetition, allowing for a smoother transition between hues.
    /// </summary>
    public bool SmoothHueBlending { get; init; } = false;

    /// <summary>
    ///     Compares two colours by computing their custom step representation and using it for sorting.
    /// </summary>
    /// <param name="a">
    ///     The first colour to compare.
    /// </param>
    /// <param name="b">
    ///     The second colour to compare.
    /// </param>
    /// <returns>
    ///     A value less than zero if <paramref name="a"/> is less than <paramref name="b"/>, zero if they are equal, 
    ///     or greater than zero if <paramref name="a"/> is greater than <paramref name="b"/>.
    /// </returns>
    public int Compare(Color a, Color b)
    {
        var c1 = Step(a);
        var c2 = Step(b);

        return ((IComparable)c1).CompareTo(c2);
    }

    /// <summary>
    ///     Computes a custom step representation of a colour, based on its hue, lightness, and brightness.
    /// </summary>
    /// <param name="color">
    ///     The colour to process.
    /// </param>
    /// <returns>
    ///     A tuple representing the quantised hue, lightness, and brightness of the colour.
    /// </returns>
    private (int H, int L, int V) Step(Color color)
    {
        int lum = (int)Math.Sqrt(.241 * color.R + .691 * color.G + .068 * color.B);

        float hue = 1 - GameMathsEx.RotateDeg(color.GetHue(), 90) / 360;
        float lightness = color.GetBrightness();

        int h2 = (int)(hue * Repetitions);
        int v2 = (int)(lightness * Repetitions);

        if (SmoothHueBlending)
        {
            if ((h2 % 2) == 0)
                v2 = Repetitions - v2;
            else
                lum = Repetitions - lum;
        }

        return (h2, lum, v2);
    }
}