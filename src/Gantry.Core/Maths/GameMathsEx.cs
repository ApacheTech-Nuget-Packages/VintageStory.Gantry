using System;
using JetBrains.Annotations;
using Vintagestory.API.MathTools;

namespace Gantry.Core.Maths
{
    /// <summary>
    ///     An even larger set of extremely useful mathematical functions.
    /// </summary>
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public static class GameMathsEx
    {
        /// <summary>
        ///     The mathematical constant, i, which is equal to the sqrt. of -1.
        /// </summary>
        public static double I { get; } = GameMath.Sqrt(-1);

        /// <summary>
        ///     Calculates the distributive product of two binomials.
        /// </summary>
        /// <param name="a">The first binomials.</param>
        /// <param name="b">The second binomials.</param>
        /// <returns>The distributive product of two binomials.</returns>
        public static double DistributiveProduct(Vec2f a, Vec2f b)
        {
            return DistributiveProduct(a.X, a.Y, b.X, b.Y);
        }

        /// <summary>
        ///     Calculates the distributive product of two binomials.
        /// </summary>
        /// <param name="a">The first binomials.</param>
        /// <param name="b">The second binomials.</param>
        /// <returns>The distributive product of two binomials.</returns>
        public static double DistributiveProduct(Vec2d a, Vec2d b)
        {
            return DistributiveProduct(a.X, a.Y, b.X, b.Y);
        }

        /// <summary>
        ///     Calculates the distributive product of two binomials.
        /// </summary>
        /// <param name="a1">The first index of the first binomial.</param>
        /// <param name="a2">The second index of the first binomial.</param>
        /// <param name="b1">The first index of the second binomial.</param>
        /// <param name="b2">The second index of the second binomial.</param>
        /// <returns>The distributive product of two binomials.</returns>
        public static double DistributiveProduct(double a1, double a2, double b1, double b2)
        {
            var first = a1 * b1;
            var outside = a1 * b2;
            var inside = a2 * b1;
            var last = a2 * b2;
            return first + outside + inside + last;
        }

        /// <summary>
        ///     Calculates the distributive product of two binomials.
        /// </summary>
        /// <param name="a1">The first index of the first binomial.</param>
        /// <param name="a2">The second index of the first binomial.</param>
        /// <param name="b1">The first index of the second binomial.</param>
        /// <param name="b2">The second index of the second binomial.</param>
        /// <returns>The distributive product of two binomials.</returns>
        public static float DistributiveProduct(float a1, float a2, float b1, float b2)
        {
            var first = a1 * b1;
            var outside = a1 * b2;
            var inside = a2 * b1;
            var last = a2 * b2;
            return first + outside + inside + last;
        }

        /// <summary>
        ///     Clamps a value between two points, by continuously wrapping the value
        ///     around the minimum and maximum points, until the value falls within range.
        ///     <br/><br/>For example, ClampWrap(14, 5, 12) = 7.
        /// </summary>
        /// <param name="val">The value to wrap.</param>
        /// <param name="min">The minimum value.</param>
        /// <param name="max">The maximum value.</param>
        /// <returns></returns>
        public static float ClampWrap(float val, float min, float max)
        {
            val -= (float)Math.Round((val - min) / (max - min)) * (max - min);
            if (val < 0)
                val = val + max - min;
            return val;
        }

        /// <summary>
        ///     Increments the array by a specified amount, for each individual index.
        /// </summary>
        /// <param name="source">The source array.</param>
        /// <param name="factor">The values to increment each index by.</param>
        /// <returns></returns>
        public static byte[] Increment(this byte[] source, Vec3f factor)
        {
            var r = (byte)GameMath.Clamp(source[0] + factor.R, 0, 255);
            var g = (byte)GameMath.Clamp(source[1] + factor.G, 0, 255);
            var b = (byte)GameMath.Clamp(source[2] + factor.B, 0, 255);
            return new[] { r, g, b };
        }

        /// <summary>
        ///     Increments the array by a specified amount, for all indices.
        /// </summary>
        /// <param name="source">The source array.</param>
        /// <param name="factor">The value to increment each index by.</param>
        public static byte[] Increment(this byte[] source, float factor)
        {
            var r = (byte)GameMath.Clamp(source[0] + factor, 0, 255);
            var g = (byte)GameMath.Clamp(source[1] + factor, 0, 255);
            var b = (byte)GameMath.Clamp(source[2] + factor, 0, 255);
            return new[] { r, g, b };
        }

        /// <summary>
        ///     Brighten a colour by a specific amount.
        /// </summary>
        /// <param name="source">The source array.</param>
        /// <param name="factor">The value to brighten each index by.</param>
        public static Vec3f Brighten(this Vec3f source, Vec3f factor)
        {
            var r = Math.Abs(source.R);
            var g = Math.Abs(source.G);
            var b = Math.Abs(source.B);
            return new Vec3f(
                (byte)GameMath.Clamp(r * factor.R, r, 255), 
                (byte)GameMath.Clamp(g * factor.G, g, 255), 
                (byte)GameMath.Clamp(b * factor.B, b, 255));
        }

        /// <summary>
        ///     ScaleBy a vector by a specific amount.
        /// </summary>
        /// <param name="source">The source array.</param>
        /// <param name="factor">The value to scale each index by.</param>
        public static Vec3f ScaleBy(this Vec3f source, Vec3f factor)
        {
            return new Vec3f(source.X * factor.X, source.Y * factor.Y, source.Z * factor.Z);
        }

        /// <summary>
        ///     ScaleBy a vector by a specific amount.
        /// </summary>
        /// <param name="source">The source array.</param>
        /// <param name="factor">The value to scale each index by.</param>
        public static Vec3d ScaleBy(this Vec3d source, Vec3d factor)
        {
            return new Vec3d(source.X * factor.X, source.Y * factor.Y, source.Z * factor.Z);
        }

        /// <summary>
        ///     ScaleBy a vector by a specific amount.
        /// </summary>
        /// <param name="source">The source array.</param>
        /// <param name="factor">The value to scale each index by.</param>
        public static Vec3f ScaleBy(this Vec3f source, double[] factor)
        {
            return new Vec3f(source.X * (float)factor[0], source.Y * (float)factor[1], source.Z * (float)factor[2]);
        }

        /// <summary>
        ///     ScaleBy a vector by a specific amount.
        /// </summary>
        /// <param name="source">The source array.</param>
        /// <param name="factor">The value to scale each index by.</param>
        public static Vec3d ScaleBy(this Vec3d source, double[] factor)
        {
            return new Vec3d(source.X * factor[0], source.Y * factor[1], source.Z * factor[2]);
        }

        /// <summary>
        ///     ScaleBy a vector by a specific amount.
        /// </summary>
        /// <param name="source">The source array.</param>
        /// <param name="factor">The value to scale each index by.</param>
        public static byte[] ScaleBy(this byte[] source, Vec3f factor)
        {
            var r = (byte)GameMath.Clamp(source[0] * factor.R, 0, 255);
            var g = (byte)GameMath.Clamp(source[1] * factor.G, 0, 255);
            var b = (byte)GameMath.Clamp(source[2] * factor.B, 0, 255);
            return new[] { r, g, b };
        }

        /// <summary>
        ///     ScaleBy a vector by a specific amount.
        /// </summary>
        /// <param name="source">The source array.</param>
        /// <param name="factor">The value to scale each index by.</param>
        public static byte[] ScaleBy(this byte[] source, float factor)
        {
            var r = (byte)GameMath.Clamp(source[0] * factor, 0, 255);
            var g = (byte)GameMath.Clamp(source[1] * factor, 0, 255);
            var b = (byte)GameMath.Clamp(source[2] * factor, 0, 255);
            return new[] { r, g, b };
        }
    }
}