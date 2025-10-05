namespace Gantry.Core.Annotation;

/// <summary>
///     Indicates that the decorated member or object is meant for used on both the client, and the server. It is up to the developer to enforce this rule.
/// </summary>
/// <seealso cref="Attribute" />
public sealed class UniversalAttribute : SidedAttribute
{
    /// <summary>
    ///  	Initialises a new instance of the <see cref="UniversalAttribute"/> class.
    /// </summary>
    public UniversalAttribute() : base(EnumAppSide.Universal) { }
}