namespace Gantry.Core.Maths.Interpolation;

/// <summary>
///     Cubic Interpolation
/// </summary>
/// <seealso cref="InterpolationBase" />
public class CubicInterpolation : InterpolationBase
{
    private readonly double _beginVec;
    private readonly double _endVec;

    /// <summary>
    ///     Initialises a new instance of the <see cref="CubicInterpolation"/> class.
    /// </summary>
    /// <param name="times">The times.</param>
    /// <param name="points">The points.</param>
    public CubicInterpolation(double[] times, double[] points) : base(times, points)
    {
        _beginVec = points[0] + (points[0] - points[1]);
        _endVec = points[points.Length - 1] + (points[points.Length - 1] - points[points.Length - 2]);
    }

    /// <summary>
    ///     Initialises a new instance of the <see cref="CubicInterpolation"/> class.
    /// </summary>
    /// <param name="points">The points.</param>
    public CubicInterpolation(params double[] points) : base(points)
    {
        _beginVec = points[0] + (points[0] - points[1]);
        _endVec = points[points.Length - 1] + (points[points.Length - 1] - points[points.Length - 2]);
    }

    /// <summary>
    ///     Gets the value at a specific node within the path.
    /// </summary>
    /// <param name="index">The index.</param>
    /// <returns></returns>
    protected override double GetValue(int index)
    {
        if (index < 0) return _beginVec;
        return index >= Points.Count
            ? _endVec
            : PointVectors[index];
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
    public override double ValueAt(double mu, int pointIndex, int pointIndexNext)
    {
        var v0 = GetValue(pointIndex - 1);
        var v1 = GetValue(pointIndex);
        var v2 = GetValue(pointIndexNext);
        var v3 = GetValue(pointIndexNext + 1);

        var mu2 = mu * mu;
        var a0 = v3 - v2 - v0 + v1;
        var a1 = v0 - v1 - a0;
        var a2 = v2 - v0;
        var a3 = v1;

        return a0 * mu * mu2 + a1 * mu2 + a2 * mu + a3;
    }
}