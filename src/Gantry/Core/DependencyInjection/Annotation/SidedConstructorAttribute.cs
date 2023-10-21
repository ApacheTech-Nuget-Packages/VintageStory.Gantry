using ApacheTech.Common.DependencyInjection.Abstractions;
using JetBrains.Annotations;
using Vintagestory.API.Common;

namespace Gantry.Core.DependencyInjection.Annotation;

/// <summary>
/// Marks the constructor to be used when activating type using <see cref="ActivatorUtilities" />.
/// </summary>
[AttributeUsage(AttributeTargets.Constructor)]
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class SidedConstructorAttribute : InjectableConstructorAttribute
{
    /// <summary>
    ///     The app-side that this instance was instantiated on.
    /// </summary>
    /// <value>The side.</value>
    public EnumAppSide Side { get; }

    /// <summary>
    /// 	Initialises a new instance of the <see cref="SidedConstructorAttribute"/> class.
    /// </summary>
    public SidedConstructorAttribute() : this(EnumAppSide.Universal)
    {
    }

    /// <summary>
    /// 	Initialises a new instance of the <see cref="SidedConstructorAttribute"/> class.
    /// </summary>
    /// <param name="side">The side.</param>
    public SidedConstructorAttribute(EnumAppSide side)
    {
        Side = side;
    }
}