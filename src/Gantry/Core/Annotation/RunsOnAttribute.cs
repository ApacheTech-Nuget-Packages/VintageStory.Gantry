using JetBrains.Annotations;
using Vintagestory.API.Common;

namespace Gantry.Core.Annotation;

/// <summary>
///     Indicates that the decorated member or object is only meant for use on a specified app side. It is up to the developer to enforce this rule.
/// </summary>
/// <seealso cref="Attribute" />
[AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = false)]
[UsedImplicitly(ImplicitUseTargetFlags.All)]
public class RunsOnAttribute : Attribute
{
    /// <summary>
    ///     Gets the acceptable app side.
    /// </summary>
    public EnumAppSide Side { get; }

    /// <summary>
    ///  	Initialises a new instance of the <see cref="RunsOnAttribute"/> class.
    /// </summary>
    /// <param name="side">The app side that this handler should be registered on.</param>
    public RunsOnAttribute(EnumAppSide side)
    {
        Side = side;
    }

    /// <summary>
    ///     Determines whether the target should run on the specified app side.
    /// </summary>
    /// <param name="side">The side.</param>
    public bool ShouldRun(EnumAppSide side)
        => Side == EnumAppSide.Universal || side == Side;
}