using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Gantry.Core.Maths.Interpolation
{
    /// <summary>
    ///     Acts as a base class for each type of interpolation.
    /// </summary>
    /// <seealso cref="IInterpolator" />
    [UsedImplicitly(ImplicitUseTargetFlags.All)]
    public abstract class InterpolationBase : IInterpolator
    {
        /// <summary>
        ///     Gets the list of key points, or nodes, along a path.
        /// </summary>
        protected Dictionary<double, double> Points { get; } = new();

        /// <summary>
        ///     Gets the list of vectors, along a path, denoting change of speed, or direction.
        /// </summary>
        protected List<double> PointVectors { get; }

        /// <summary>
        ///     Initialises a new instance of the <see cref="InterpolationBase"/> class.
        /// </summary>
        /// <param name="times">The times.</param>
        /// <param name="points">The points.</param>
        /// <exception cref="ArgumentException">
        /// At least two points are needed! - points
        /// or
        /// Invalid times array! - times
        /// </exception>
        protected InterpolationBase(double[] times, double[] points)
        {
            if (points.Length < 2)
                throw new ArgumentException("At least two points are needed!", nameof(points));

            if (times.Length != points.Length)
                throw new ArgumentException("Invalid times array!", nameof(times));

            for (var i = 0; i < points.Length; i++) Points.Add(times[i], points[i]);
            PointVectors = new List<double>(points);
        }

        /// <summary>
        ///     Initialises a new instance of the <see cref="InterpolationBase"/> class.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <exception cref="System.ArgumentException">At least two points are needed! - points</exception>
        protected InterpolationBase(params double[] points)
        {
            if (points.Length < 2)
                throw new ArgumentException("At least two points are needed!", nameof(points));

            double time = 0;
            var stepLength = 1D / (points.Length - 1);
            foreach (var t in points)
            {
                Points.Add(time, t);
                time += stepLength;
            }

            PointVectors = new List<double>(points);
        }

        /// <summary>
        ///     Calculates the value at a specific point between two nodes.
        /// </summary>
        /// <param name="mu">The fractional point between with two nodes.</param>
        /// <param name="pointIndex">The start node index.</param>
        /// <param name="pointIndexNext">The end node index.</param>
        /// <returns>
        ///     A <see cref="double" /> value that represents a specific point along a curve between two nodes of a path.
        /// </returns>
        public abstract double ValueAt(double mu, int pointIndex, int pointIndexNext);

        /// <summary>
        ///     Gets the value at a specific node within the path.
        /// </summary>
        ///     <param name="index">The index.</param>
        /// <returns></returns>
        protected virtual double GetValue(int index)
        {
            return PointVectors[index];
        }


        /// <summary>
        ///     Calculates the value at a specific point along the overall path.
        /// </summary>
        /// <param name="point">The fractional point, between 0..1 along the overall path.</param>
        /// <returns>
        ///     A <see cref="double" /> value that represents a specific point along the overall path.
        /// </returns>
        public double ValueAt(double point)
        {
            if (!(point >= 0) || !(point <= 1)) return default;
            KeyValuePair<double, double> firstPoint = default;
            var indexFirst = -1;

            KeyValuePair<double, double> secondPoint = default;
            var indexSecond = -1;

            var i = 0;
            foreach (var entry in Points)
            {
                if (entry.Key >= point)
                {
                    if (firstPoint.Equals(default(KeyValuePair<double, double>)))
                    {
                        firstPoint = entry;
                        indexFirst = i;
                    }
                    else
                    {
                        secondPoint = entry;
                        indexSecond = i;
                    }

                    break;
                }

                firstPoint = entry;
                indexFirst = i;
                i++;
            }

            if (secondPoint.Equals(default(KeyValuePair<double, double>)))
                return firstPoint.Value;

            var pointDistance = secondPoint.Key - firstPoint.Key;
            var mu = (point - firstPoint.Key) / pointDistance;
            return ValueAt(mu, indexFirst, indexSecond);
        }
    }
}