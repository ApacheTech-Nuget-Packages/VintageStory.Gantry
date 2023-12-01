using System.Drawing;
using ApacheTech.Common.Extensions.System;
using Gantry.Core.GameContent.AssetEnum;
using JetBrains.Annotations;
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
}