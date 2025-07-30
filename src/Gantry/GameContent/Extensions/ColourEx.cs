using Vintagestory.API.MathTools;

namespace Gantry.GameContent.Extensions;

/// <summary>
///     Provides extended colour utilities, including methods for generating random colours and clamped random colours.
/// </summary>
public class ColourEx : ColorUtil
{
    /// <summary>
    ///     Generates a random colour in HSV format and converts it to an RGB integer representation.
    /// </summary>
    /// <param name="api">The core API instance providing access to the world and random number generator.</param>
    /// <returns>An integer representing the RGB colour value.</returns>
    public static int RandomColour(ICoreAPI api)
        => HsvToRgb(
            (int)(api.World.Rand.NextDouble() * 255),
            (int)(api.World.Rand.NextDouble() * 255),
            (int)(api.World.Rand.NextDouble() * 255)
        );

    /// <summary>
    ///     Generates a random colour in HSV format, converts it to an RGB representation, and returns it as a hexadecimal string.
    /// </summary>
    /// <param name="api">The core API instance providing access to the world and random number generator.</param>
    /// <returns>A string representing the RGB colour value in hexadecimal format.</returns>
    public static string RandomHexColour(ICoreAPI api)
        => RandomColour(api).ToString("X");

    /// <summary>
    ///     Generates a random colour in HSV format, clamps the brightness value to a specified range, converts it to an RGB 
    ///     representation, and returns it as a hexadecimal string.
    /// </summary>
    /// <param name="api">The core API instance providing access to the world and random number generator.</param>
    /// <param name="min">The minimum brightness value (0.0 to 1.0).</param>
    /// <param name="max">The maximum brightness value (0.0 to 1.0).</param>
    /// <returns>A string representing the clamped RGB colour value in hexadecimal format.</returns>
    public static string RandomClampedHexColour(ICoreAPI api, double min, double max)
        => ClampedRandomColourValue(api, min, max).ToString("X");

    /// <summary>
    ///     Generates a random colour in HSV format with the brightness value clamped to a specified range and converts it to 
    ///     an RGB integer representation.
    /// </summary>
    /// <param name="api">The core API instance providing access to the world and random number generator.</param>
    /// <param name="min">The minimum brightness value (0.0 to 1.0).</param>
    /// <param name="max">The maximum brightness value (0.0 to 1.0).</param>
    /// <returns>An integer representing the clamped RGB colour value.</returns>
    public static int ClampedRandomColourValue(ICoreAPI api, double min, double max)
        => HsvToRgb(
            (int)(api.World.Rand.NextDouble() * 255),
            (int)(api.World.Rand.NextDouble() * 255),
            (int)(GameMath.Clamp(api.World.Rand.NextDouble(), min, max) * 255)
        );
}