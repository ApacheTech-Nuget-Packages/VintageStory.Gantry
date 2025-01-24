using System.Drawing;
using Gantry.Core.GameContent.AssetEnum;
using Vintagestory.API.MathTools;

namespace Gantry.Core.Extensions;

/// <summary>
///     Extension methods to aid conversion of values to colour representations.
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class ColourExtensions
{
    /// <summary>
    ///     Converts an array of <see cref="double"/> values to a <see cref="Vec4f"/> in RGBA format.
    /// </summary>
    public static Vec4f ToRgbaVec4F(this double[] rgba)
    {
        return new Vec4f((float)rgba[0], (float)rgba[1], (float)rgba[2], (float)rgba[3]);
    }

    /// <summary>
    ///     Converts an array of <see cref="float"/> values to a <see cref="Vec4f"/> in RGBA format.
    /// </summary>
    public static Vec4f ToRgbaVec4F(this float[] rgba)
    {
        return new Vec4f(rgba[0], rgba[1], rgba[2], rgba[3]);
    }

    /// <summary>
    ///     Converts a <see cref="Color"/> to a <see cref="Vec4f"/> in RGBA format.
    /// </summary>
    public static Vec4f ToRgbaVec4F(this Color colour)
    {
        return new Vec4f(colour.R, colour.G, colour.B, colour.A);
    }

    /// <summary>
    ///     Converts a <see cref="string"/> to a <see cref="Vec4f"/> in RGBA format.
    /// </summary>
    public static Vec4f ToRgbaVec4F(this string colourString)
    {
        var colour = colourString.ToColour();
        return new Vec4f(colour.R, colour.G, colour.B, colour.A);
    }

    /// <summary>
    ///     Converts a <see cref="NamedColour"/> to a <see cref="Vec4f"/> in RGBA format.
    /// </summary>
    public static Vec4f ToRgbaVec4F(this NamedColour colourString)
    {
        var colour = colourString.ToString()!.ToColour();
        return new Vec4f(colour.R, colour.G, colour.B, colour.A);
    }

    /// <summary>
    ///     Converts a colour to its HSV representation as a byte array.
    /// </summary>
    /// <param name="colour">The colour to convert.</param>
    /// <returns>An array containing the HSV values scaled to 0–255.</returns>
    public static byte[] ToHsvByteArray(this Color colour)
    {
        var r = colour.R / 255.0;
        var g = colour.G / 255.0;
        var b = colour.B / 255.0;

        var max = Math.Max(r, Math.Max(g, b));
        var min = Math.Min(r, Math.Min(g, b));
        var delta = max - min;

        var h = 0.0;
        if (delta > 0)
        {
            if (max == r)
            {
                h = 60 * (((g - b) / delta) % 6);
            }
            else if (max == g)
            {
                h = 60 * (((b - r) / delta) + 2);
            }
            else if (max == b)
            {
                h = 60 * (((r - g) / delta) + 4);
            }
        }
        if (h < 0)
        {
            h += 360;
        }

        var s = max == 0 ? 0 : (delta / max);
        var v = max;

        var hue = (byte)(h / 360 * 255);
        var saturation = (byte)(s * 255);
        var vibrance = (byte)(v * 255);

        return [hue, saturation, vibrance];
    }
}