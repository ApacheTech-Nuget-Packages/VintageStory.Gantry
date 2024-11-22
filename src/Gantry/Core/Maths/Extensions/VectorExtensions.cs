using Vintagestory.API.MathTools;

namespace Gantry.Core.Maths.Extensions;

/// <summary>
///     Provides extension methods for vector and scalar operations, including normalisation and vector type conversions.
/// </summary>
public static class VectorExtensions
{
    /// <summary>
    ///     Normalises a float value to -1, 0, or 1 based on its sign.
    /// </summary>
    /// <param name="value">The float value to normalise.</param>
    /// <returns>The normalised value, or 0 if input is 0.</returns>
    public static float Normalise(this float value)
    {
        return value == 0 ? 0 : value / Math.Abs(value);
    }

    /// <summary>
    ///     Normalises a double value to -1, 0, or 1 based on its sign.
    /// </summary>
    /// <param name="value">The double value to normalise.</param>
    /// <returns>The normalised value, or 0 if input is 0.</returns>
    public static double Normalise(this double value)
    {
        return value == 0 ? 0 : value / Math.Abs(value);
    }

    /// <summary>
    ///     Converts a <see cref="Vec3d"/> instance to a <see cref="Vec3i"/> instance by flooring each component.
    /// </summary>
    /// <param name="vector">The <see cref="Vec3d"/> to convert.</param>
    /// <returns>A new <see cref="Vec3i"/> with integer components.</returns>
    public static Vec3i ToVec3i(this Vec3d vector)
    {
        return new Vec3i(vector.AsBlockPos);
    }

    /// <summary>
    ///     Converts a <see cref="Vec3f"/> instance to a <see cref="Vec3i"/> instance by converting it to a <see cref="Vec3d"/> first.
    /// </summary>
    /// <param name="vector">The <see cref="Vec3f"/> to convert.</param>
    /// <returns>A new <see cref="Vec3i"/> with integer components.</returns>
    public static Vec3i ToVec3i(this Vec3f vector)
    {
        return new Vec3i(vector.ToVec3d().AsBlockPos);
    }
}
