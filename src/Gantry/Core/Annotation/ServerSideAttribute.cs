namespace Gantry.Core.Annotation;

/// <summary>
///     Indicates that the decorated member or object is only meant for used on the server. It is up to the developer to enforce this rule.
/// </summary>
/// <seealso cref="Attribute" />
public sealed class ServerSideAttribute : RunsOnAttribute
{
    /// <summary>
    ///  	Initialises a new instance of the <see cref="ServerSideAttribute"/> class.
    /// </summary>
    public ServerSideAttribute() : base(EnumAppSide.Server) { }
}