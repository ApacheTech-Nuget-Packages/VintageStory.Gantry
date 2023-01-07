using JetBrains.Annotations;
using System;
using System.Threading;

namespace Gantry.Core.Extensions.Helpers
{
    /// <summary>
    ///     Static Helper class for Random Number Generation.
    /// </summary>
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public static class RandomEx
    {
        private static ThreadLocal<Random> _random = new();

        /// <summary>
        ///     Returns a shared instance of <see cref="Random"/> that can safely be used across threads.
        /// </summary>
        public static Random Shared => _random.Value ??= new Random((int)
            ((1 + Thread.CurrentThread.ManagedThreadId) * DateTime.UtcNow.Ticks));

        /// <summary>
        ///     Returns a random <see langword="double"/> that is within a specified range.
        /// </summary>
        /// <param name="minValue">The inclusive lower bound of the random number returned.</param>
        /// <param name="maxValue">The exclusive upper bound of the random number returned. maxValue must be greater than or equal to minValue.</param>
        /// <returns>A double-precision floating-point number greater than or equal to <paramref name="minValue">minValue</paramref> and less than <paramref name="maxValue">maxValue</paramref>; that is, the range of return values includes <paramref name="minValue">minValue</paramref> but not <paramref name="maxValue">maxValue</paramref>. If <paramref name="minValue">minValue</paramref> equals <paramref name="maxValue">maxValue</paramref>, <paramref name="minValue">minValue</paramref> is returned.</returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="minValue">minValue</paramref> is greater than <paramref name="maxValue">maxValue</paramref>.</exception>
        public static double RandomValueBetween(double minValue, double maxValue)
        {
            if (minValue.Equals(maxValue)) return minValue;
            if (minValue > maxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(minValue), minValue,
                    "The minimum value is greater than the maximum value.");
            }
            return Shared.NextDouble() * (maxValue - minValue) + minValue;
        }

        /// <summary>
        ///     Returns a random <see langword="double"/> that is within a specified range.
        /// </summary>
        /// <param name="value">The value to randomise.</param>
        /// <param name="threshold">The threshold of the lower and upper bounds of randomisation.</param>
        public static double RandomValueAround(double value, double threshold)
        {
            return RandomValueBetween(value - threshold, value + threshold);
        }

        /// <summary>
        ///     Returns a random <see langword="float"/> that is within a specified range.
        /// </summary>
        /// <param name="value">The value to randomise.</param>
        /// <param name="threshold">The threshold of the lower and upper bounds of randomisation.</param>
        public static float RandomValueAround(float value, float threshold)
        {
            return (float)RandomValueBetween(value - threshold, value + threshold);
        }

        /// <summary>
        ///     Returns a random <see langword="int"/> that is within a specified range.
        /// </summary>
        /// <param name="value">The value to randomise.</param>
        /// <param name="threshold">The threshold of the lower and upper bounds of randomisation.</param>
        public static int RandomValueAround(int value, int threshold)
        {
            return (int)RandomValueBetween(value - threshold, value + threshold);
        }

        /// <summary>
        ///     Returns a random <see langword="bool"/> value.
        /// </summary>
        public static bool RandomBool()
        {
            return Shared.Next(2) == 0;
        }

        /// <summary>
        ///     Returns a <see langword="double"/> with a random signing.
        ///     The same numerical value as the input, but either positive, or negative.
        /// </summary>
        /// <param name="value">The value.</param>
        public static double RandomSign(double value)
        {
            return RandomBool() ? value : -value;
        }

        /// <summary>
        ///     Returns a <see langword="double"/> with a random signing.
        ///     The same numerical value as the input, but either positive, or negative.
        /// </summary>
        /// <param name="value">The value.</param>
        public static float RandomSign(float value)
        {
            return RandomBool() ? value : -value;
        }

        /// <summary>
        ///     Returns a <see langword="double"/> with a random signing.
        ///     The same numerical value as the input, but either positive, or negative.
        /// </summary>
        /// <param name="value">The value.</param>
        public static int RandomSign(int value)
        {
            return RandomBool() ? value : -value;
        }

        /// <summary>
        ///     Returns a <see langword="double"/> with a random signing.
        ///     The same numerical value as the input, but either positive, or negative.
        /// </summary>
        /// <param name="value">The value.</param>
        public static long RandomSign(long value)
        {
            return RandomBool() ? value : -value;
        }
    }
}