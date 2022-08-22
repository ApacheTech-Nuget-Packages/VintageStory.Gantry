using JetBrains.Annotations;
using Vintagestory.API.MathTools;

namespace Gantry.Core.Maths.Extensions
{
    /// <summary>
    ///     Extension methods for dealing with vectors.
    /// </summary>
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public static class Vec3dExtensions
    {
        /// <summary>
        ///     Scales the vector by the specified scale factor.
        /// </summary>
        /// <param name="vec">The vector to scale.</param>
        /// <param name="scaleFactor">The scale factor.</param>
        /// <returns>A new instance of <see cref="Vec3d"/>, scaled from the original, by the given factor.</returns>
        public static Vec3d Scale(this Vec3d vec, double scaleFactor)
        {
            return new Vec3d(
                vec.X * scaleFactor,
                vec.Y * scaleFactor,
                vec.Z * scaleFactor);
        }
    }
}