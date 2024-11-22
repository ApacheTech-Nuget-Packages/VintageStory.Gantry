using Vintagestory.API.MathTools;

namespace Gantry.Core.Maths.Helpers;

/// <summary>
///     Provides utility methods and properties for 3D vector operations.
/// </summary>
public static class Vec3dEx
{
    /// <summary>
    ///     Gets the unit vector along the X-axis (1, 0, 0).
    /// </summary>
    public static Vec3d UnitX => new(1, 0, 0);

    /// <summary>
    ///     Gets the unit vector along the Y-axis (0, 1, 0).
    /// </summary>
    public static Vec3d UnitY => new(0, 1, 0);

    /// <summary>
    ///     Gets the unit vector along the Z-axis (0, 0, 1).
    /// </summary>
    public static Vec3d UnitZ => new(0, 0, 1);
}