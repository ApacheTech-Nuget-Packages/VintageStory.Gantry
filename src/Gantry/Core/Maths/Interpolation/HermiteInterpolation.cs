using Gantry.Core.Maths.Enum;

namespace Gantry.Core.Maths.Interpolation;

/// <summary>
///     In numerical analysis, Hermite interpolation, named after Charles Hermite, is a method of polynomial 
///     interpolation, which generalizes Lagrange interpolation. Lagrange interpolation allows computing a
///     polynomial of degree less than n that takes the same value at n given points as a given function. 
///     
///     Instead, Hermite interpolation computes a polynomial of degree less than mn such that the
///     polynomial and its m − 1 first derivatives have the same values at n given points as a given
///     function and its m − 1 first derivatives. 
///     
///     Hermite's method of interpolation is closely related to the Newton's interpolation method,
///     in that both are derived from the calculation of divided differences. However, there other
///     methods for computing a Hermite interpolating polynomial.One can use linear algebra, by taking
///     the coefficients of the interpolating polynomial as unknowns, and writing as linear equations
///     the constraints that the interpolating polynomial must satisfy.
/// </summary>
/// <remarks>For more information: https://en.wikipedia.org/wiki/Hermite_interpolation</remarks>
/// <seealso cref="InterpolationBase" />
public class HermiteInterpolation : InterpolationBase
{
    private readonly double _bias;
    private readonly HermiteTension _tension;

    /// <summary>
    ///     Initialises a new instance of the <see cref="HermiteInterpolation"/> class.
    /// </summary>
    /// <param name="times">How changes in speed and direction, as we traverse the path.</param>
    /// <param name="points">The points that dictate the angle of the curves within a path.</param>
    /// <param name="bias">What is the direction of the curve as it passes through the key-point?</param>
    /// <param name="tension">How sharply does the curve bend?</param>
    public HermiteInterpolation(double[] times, double[] points, double bias, HermiteTension tension) : base(times,
        points)
    {
        _bias = bias;
        _tension = tension;
    }

    /// <summary>
    ///     Initialises a new instance of the <see cref="HermiteInterpolation"/> class.
    /// </summary>
    /// <param name="bias">What is the direction of the curve as it passes through the key-point?</param>
    /// <param name="tension">How sharply does the curve bend?</param>
    /// <param name="points">The points that dictate the angle of the curves within a path.</param>
    public HermiteInterpolation(double bias, HermiteTension tension, params double[] points) : base(points)
    {
        _bias = bias;
        _tension = tension;
    }

    /// <summary>
    ///     Initialises a new instance of the <see cref="HermiteInterpolation"/> class.
    /// </summary>
    /// <param name="times">How changes in speed and direction, as we traverse the path.</param>
    /// <param name="points">The points that dictate the angle of the curves within a path.</param>
    /// <param name="tension">How sharply does the curve bend?</param>
    public HermiteInterpolation(double[] times, double[] points,
        HermiteTension tension = HermiteTension.Normal) : this(times, points, 0, tension)
    {
    }

    /// <summary>
    ///     Initialises a new instance of the <see cref="HermiteInterpolation"/> class.
    /// </summary>
    /// <param name="tension">How sharply does the curve bend?</param>
    /// <param name="points">The points that dictate the angle of the curves within a path.</param>
    public HermiteInterpolation(HermiteTension tension, params double[] points) : this(0, tension, points)
    {
    }

    /// <summary>
    ///     Initialises a new instance of the <see cref="HermiteInterpolation"/> class.
    /// </summary>
    /// <param name="points">The points that dictate the angle of the curves within a path.</param>
    public HermiteInterpolation(params double[] points) : this(HermiteTension.Normal, points)
    {
    }

    /// <summary>
    ///     Calculates the value at a specific point between two nodes.
    /// </summary>
    /// <param name="mu">The fractional point between with two nodes.</param>
    /// <param name="pointIndex">The start node index.</param>
    /// <param name="pointIndexNext">The end node index.</param>
    /// <returns>A <see cref="double"/> value that represents a specific point along a curve between two nodes of a path.</returns>
    public override double ValueAt(double mu, int pointIndex, int pointIndexNext)
    {
        var v0 = GetValue(pointIndex - 1);
        var v1 = GetValue(pointIndex);
        var v2 = GetValue(pointIndexNext);
        var v3 = GetValue(pointIndexNext + 1);

        var mu2 = mu * mu;
        var mu3 = mu2 * mu;
        var m0 = (v1 - v0) * (1 + _bias) * (1 - (int) _tension) / 2;
        m0 += (v2 - v1) * (1 - _bias) * (1 - (int) _tension) / 2;
        var m1 = (v2 - v1) * (1 + _bias) * (1 - (int) _tension) / 2;
        m1 += (v3 - v2) * (1 - _bias) * (1 - (int) _tension) / 2;
        var a0 = 2 * mu3 - 3 * mu2 + 1;
        var a1 = mu3 - 2 * mu2 + mu;
        var a2 = mu3 - mu2;
        var a3 = -2 * mu3 + 3 * mu2;

        return a0 * v1 + a1 * m0 + a2 * m1 + a3 * v2;
    }
}