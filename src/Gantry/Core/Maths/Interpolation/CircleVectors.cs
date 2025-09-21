using Vintagestory.API.MathTools;

namespace Gantry.Core.Maths.Interpolation;

/// <summary>
///     Defines the vectors of left and right moving circles.
/// </summary>
public class CircleVectors
{
    private static readonly double _sqrt2_2 = Math.Sqrt(2) / 2;

    /// <summary>
    ///     Defines the vectors for the right-moving circle.
    /// </summary>
    public static Vec3d[] RightMovingCircle { get; } =
    [
        new(1, 0, 0),
        new(_sqrt2_2, 0, -_sqrt2_2),
        new(0, 0, -1),
        new(-_sqrt2_2, 0, -_sqrt2_2),
        new(-1, 0, 0),
        new(-_sqrt2_2, 0, _sqrt2_2),
        new(0, 0, 1),
        new(_sqrt2_2, 0, _sqrt2_2)
    ];

    /// <summary>
    ///     Defines the vectors for the left-moving circle.
    /// </summary>
    public static Vec3d[] LeftMovingCircle { get; } =
    [
        new(1, 0, 0),
        new(_sqrt2_2, 0, _sqrt2_2),
        new(0, 0, 1),
        new(-_sqrt2_2, 0, _sqrt2_2),
        new(-1, 0, 0),
        new(-_sqrt2_2, 0, -_sqrt2_2),
        new(0, 0, -1),
        new(_sqrt2_2, 0, -_sqrt2_2)
    ];
}