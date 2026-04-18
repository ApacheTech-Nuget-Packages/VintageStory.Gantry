namespace Gantry.Services.HarmonyPatches.Annotations;

/// <summary>
///     Indicates that the decorated class contains Harmony patches, and should be processed by the patch loader.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class ContainsPatchesAttribute : Attribute, IConditionalOnSide
{
    /// <summary>
    ///     Initialises a new instance of the <see cref="ContainsPatchesAttribute"/> class for the specified application side.
    /// </summary>
    /// <param name="side">The application side to which the attribute applies. The default is EnumAppSide.Universal.</param>
    public ContainsPatchesAttribute(EnumAppSide side = EnumAppSide.Universal) => Side = side;

    /// <summary>
    ///     The application side represented by this instance.
    /// </summary>
    public EnumAppSide Side { get; }
}
