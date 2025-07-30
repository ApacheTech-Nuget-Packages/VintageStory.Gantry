namespace Gantry.Services.HarmonyPatches.Annotations;

/// <summary>
///     Denotes that the decorated object requires a specific mod to be enabled within the gameworld.
///     It is up to the developer to enforce this restriction.
/// </summary>
[AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
public sealed class RequiresModAttribute : Attribute
{
    /// <summary>
    ///  	Initialises a new instance of the <see cref="RequiresModAttribute"/> class.
    /// </summary>
    /// <param name="modId">The mod identifier of the required mod.</param>
    public RequiresModAttribute(string modId) => ModId = modId;

    /// <summary>
    ///     The ModID of the required mod.
    /// </summary>
    public string ModId { get; }
}