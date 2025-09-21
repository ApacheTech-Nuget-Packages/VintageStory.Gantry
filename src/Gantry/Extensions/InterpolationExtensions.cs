using Vintagestory.API.MathTools;

namespace Gantry.Extensions;

/// <summary>
///     Extension methods for interpolation-related calculations.
/// </summary>
public static class InterpolationExtensions
{
    /// <summary>
    ///     Adjusts the yaw value so that interpolation across the 0 / 2&amp;pi boundary behaves
    ///     correctly. This prevents large jumps when yaw angles cross the circular wrap point.
    /// </summary>
    /// <param name="previousYaw">The previous yaw value in radians.</param>
    /// <param name="currentYaw">The current yaw value in radians.</param>
    /// <returns>The normalised current yaw that avoids large discontinuities.</returns>
    public static double FixYaw(this double previousYaw, double currentYaw)
    {
        var delta = currentYaw - previousYaw;
        return delta > GameMath.PI
            ? currentYaw - GameMath.TWOPI
            : delta < -GameMath.PI
                ? currentYaw + GameMath.TWOPI
                : currentYaw;
    }
}