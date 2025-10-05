namespace Gantry.Core.Annotation;

/// <summary>
///     Indicates that the decorated member or object is only meant for used on the client. It is up to the developer to enforce this rule.
/// </summary>
/// <seealso cref="Attribute" />
public sealed class ClientSideAttribute : SidedAttribute
{
    /// <summary>
    ///  	Initialises a new instance of the <see cref="ClientSideAttribute"/> class.
    /// </summary>
    public ClientSideAttribute() : base(EnumAppSide.Client) { }
}