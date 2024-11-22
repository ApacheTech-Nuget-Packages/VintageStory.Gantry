using Vintagestory.API.MathTools;

namespace Gantry.Core.Maths.Helpers;

/// <summary>
///     Provides utility methods and properties for 2D vector operations.
/// </summary>
public static class Vec2iEx
{
    /// <summary>
    ///     Gets the unit vector along the X-axis (1, 0).
    /// </summary>
    public static Vec2i UnitX => new(1, 0);

    /// <summary>
    ///     Gets the unit vector along the Y-axis (0, 1).
    /// </summary>
    public static Vec2i UnitY => new(0, 1);
}