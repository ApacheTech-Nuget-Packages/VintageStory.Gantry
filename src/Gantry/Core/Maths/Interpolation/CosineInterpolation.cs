namespace Gantry.Core.Maths.Interpolation;

/// <summary>
///     Cosine Interpolation
/// </summary>
/// <seealso cref="InterpolationBase" />
public class CosineInterpolation : InterpolationBase
{
    /// <summary>
    ///     Initialises a new instance of the <see cref="CosineInterpolation"/> class.
    /// </summary>
    /// <param name="times">The times.</param>
    /// <param name="points">The points.</param>
    public CosineInterpolation(double[] times, double[] points) : base(times, points)
    {
    }

    /// <summary>
    ///     Initialises a new instance of the <see cref="CosineInterpolation"/> class.
    /// </summary>
    /// <param name="points">The points.</param>
    public CosineInterpolation(params double[] points) : base(points)
    {
    }

    /// <summary>
    ///     Calculates the value at a specific point between two nodes.
    /// </summary>
    /// <param name="mu">The fractional point between with two nodes.</param>
    /// <param name="pointIndex">The start node index.</param>
    /// <param name="pointIndexNext">The end node index.</param>
    /// <returns>
    /// A <see cref="double" /> value that represents a specific point along a curve between two nodes of a path.
    /// </returns>
    public override double ValueAt(double mu, int pointIndex, int pointIndexNext)
    {
        var mu2 = (1 - Math.Cos(mu * Math.PI)) / 2;
        return GetValue(pointIndex) * (1 - mu2) + GetValue(pointIndexNext) * mu2;
    }
}