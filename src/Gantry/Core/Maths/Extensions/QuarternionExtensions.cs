using OpenTK.Mathematics;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;

namespace Gantry.Core.Maths.Extensions;

/// <summary>
///     Provides extension methods for working with quaternions and Euler angles.
/// </summary>
public static class QuaternionExtensions
{
    /// <summary>
    ///     Converts a quaternion to a <see cref="Vec3d"/> representing Euler angles (pitch, yaw, roll).
    /// </summary>
    /// <param name="q">The quaternion to convert.</param>
    /// <returns>A <see cref="Vec3d"/> representing the Euler angles.</returns>
    public static Vec3d ToVec3d(this Quaternion q)
    {
        var roll = Math.Atan2(2 * (q.W * q.X + q.Y * q.Z), 1 - 2 * (q.X * q.X + q.Y * q.Y));
        var pitch = Math.Asin(2 * (q.W * q.Y - q.Z * q.X));
        var yaw = Math.Atan2(2 * (q.W * q.Z + q.X * q.Y), 1 - 2 * (q.Y * q.Y + q.Z * q.Z));
        return new Vec3d(pitch, yaw, roll);
    }

    /// <summary>
    ///     Converts a quaternion to a <see cref="Vec3f"/> representing Euler angles (pitch, yaw, roll).
    /// </summary>
    /// <param name="q">The quaternion to convert.</param>
    /// <returns>A <see cref="Vec3f"/> representing the Euler angles.</returns>
    public static Vec3f ToVec3f(this Quaternion q) => q.ToVec3d().ToVec3f();

    /// <summary>
    ///     Converts a quaternion to a <see cref="Vec3i"/> representing Euler angles (pitch, yaw, roll).
    /// </summary>
    /// <param name="q">The quaternion to convert.</param>
    /// <returns>A <see cref="Vec3i"/> representing the Euler angles.</returns>
    public static Vec3i ToVec3i(this Quaternion q) => q.ToVec3d().AsVec3i;

    /// <summary>
    ///     Converts a quaternion to a <see cref="BlockPos"/>.
    /// </summary>
    /// <param name="q">The quaternion to convert.</param>
    /// <returns>A <see cref="BlockPos"/> instance.</returns>
    public static BlockPos ToBlockPos(this Quaternion q) => q.ToVec3d().AsBlockPos;

    /// <summary>
    ///     Converts Euler angles to a quaternion.
    /// </summary>
    /// <param name="eulerAngles">The Euler angles to convert.</param>
    /// <returns>A quaternion representing the specified Euler angles.</returns>
    public static Quaternion ToQuaternion(this EntityPos eulerAngles) =>
        FromEulerAngles(eulerAngles.X, eulerAngles.Y, eulerAngles.Z);

    /// <summary>
    ///     Converts Euler angles to a quaternion.
    /// </summary>
    /// <param name="eulerAngles">The Euler angles to convert.</param>
    /// <returns>A quaternion representing the specified Euler angles.</returns>
    public static Quaternion ToQuaternion(this BlockPos eulerAngles) =>
        FromEulerAngles(eulerAngles.X, eulerAngles.Y, eulerAngles.Z);

    /// <summary>
    ///     Converts Euler angles to a quaternion.
    /// </summary>
    /// <param name="eulerAngles">The Euler angles to convert.</param>
    /// <returns>A quaternion representing the specified Euler angles.</returns>
    public static Quaternion ToQuaternion(this Vec3i eulerAngles) =>
        FromEulerAngles(eulerAngles.X, eulerAngles.Y, eulerAngles.Z);

    /// <summary>
    ///     Converts Euler angles to a quaternion.
    /// </summary>
    /// <param name="eulerAngles">The Euler angles to convert.</param>
    /// <returns>A quaternion representing the specified Euler angles.</returns>
    public static Quaternion ToQuaternion(this Vec3d eulerAngles) =>
        FromEulerAngles(eulerAngles.X, eulerAngles.Y, eulerAngles.Z);

    /// <summary>
    ///     Converts Euler angles to a quaternion.
    /// </summary>
    /// <param name="eulerAngles">The Euler angles to convert.</param>
    /// <returns>A quaternion representing the specified Euler angles.</returns>
    public static Quaternion ToQuaternion(this Vec3f eulerAngles) =>
        FromEulerAngles(eulerAngles.X, eulerAngles.Y, eulerAngles.Z);

    /// <summary>
    ///     Converts Euler angles (pitch, yaw, roll) to a quaternion.
    /// </summary>
    /// <param name="pitch">The pitch angle in radians.</param>
    /// <param name="yaw">The yaw angle in radians.</param>
    /// <param name="roll">The roll angle in radians.</param>
    /// <returns>A quaternion representing the specified Euler angles.</returns>
    private static Quaternion FromEulerAngles(double pitch, double yaw, double roll)
    {
        var rollOver2 = roll * 0.5;
        var sinRollOver2 = Math.Sin(rollOver2);
        var cosRollOver2 = Math.Cos(rollOver2);

        var pitchOver2 = pitch * 0.5;
        var sinPitchOver2 = Math.Sin(pitchOver2);
        var cosPitchOver2 = Math.Cos(pitchOver2);

        var yawOver2 = yaw * 0.5;
        var sinYawOver2 = Math.Sin(yawOver2);
        var cosYawOver2 = Math.Cos(yawOver2);

        return new()
        {
            W = (float)(cosYawOver2 * cosPitchOver2 * cosRollOver2 + sinYawOver2 * sinPitchOver2 * sinRollOver2),
            X = (float)(cosYawOver2 * cosPitchOver2 * sinRollOver2 - sinYawOver2 * sinPitchOver2 * cosRollOver2),
            Y = (float)(cosYawOver2 * sinPitchOver2 * cosRollOver2 + sinYawOver2 * cosPitchOver2 * sinRollOver2),
            Z = (float)(sinYawOver2 * cosPitchOver2 * cosRollOver2 - cosYawOver2 * sinPitchOver2 * sinRollOver2)
        };
    }
}