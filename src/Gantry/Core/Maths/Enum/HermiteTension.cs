namespace Gantry.Core.Maths.Enum;

/// <summary>
///     How sharply does the curve bend?
///     The tension is used to calculate the tangents, which must be in the interval [-1, 1].
///     In some sense, this can be interpreted as the "length" of the tangent.
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public enum HermiteTension
{
    /// <summary>
    ///     High tension yields all zero tangents.
    /// </summary>
    High = 1,

    /// <summary>
    ///     Produces a spline much closer to a Catmull–Rom spline.
    /// </summary>
    Normal = 0,

    /// <summary>
    ///     Low tension yields all one tangents.
    /// </summary>
    Low = -1
}