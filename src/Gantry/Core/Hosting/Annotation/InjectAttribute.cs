namespace Gantry.Core.Hosting.Annotation;

/// <summary>
///     Indicates that a property should be injected by the dependency injection container.
/// </summary>
[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public class InjectAttribute(EnumAppSide side = EnumAppSide.Universal) : Attribute
{
    /// <summary>
    ///     The application side for which the property should be injected.
    /// </summary>
    public EnumAppSide Side { get; set; } = side;
}