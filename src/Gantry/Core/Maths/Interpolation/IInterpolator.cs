using JetBrains.Annotations;

namespace Gantry.Core.Maths.Interpolation;

/// <summary>
///     Represents a specific form of interpolation between two nodes within a path.
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public interface IInterpolator
{
    /// <summary>
    ///     Calculates the value at a specific point between two nodes.
    /// </summary>
    /// <param name="mu">The fractional point between with two nodes.</param>
    /// <param name="pointIndex">The start node index.</param>
    /// <param name="pointIndexNext">The end node index.</param>
    /// <returns>A <see cref="double"/> value that represents a specific point along a curve between two nodes of a path.</returns>
    double ValueAt(double mu, int pointIndex, int pointIndexNext);
}