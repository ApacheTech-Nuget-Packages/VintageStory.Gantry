namespace Gantry.Core.Annotation;

/// <summary>
///     Indicates that the decorated member or object is only meant for use on a specified app side. It is up to the developer to enforce this rule.
/// </summary>
/// <seealso cref="Attribute" />
[AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = false)]
public class SidedAttribute(EnumAppSide side = EnumAppSide.Universal) : Attribute
{
    /// <summary>
    ///     Gets the acceptable app side.
    /// </summary>
    public EnumAppSide Side { get; } = side;

    /// <summary>
    ///     Determines whether the target should run on the specified app side.
    /// </summary>
    /// <param name="side">The side.</param>
    public bool For(EnumAppSide side)
        => Side == EnumAppSide.Universal || side == Side;
}