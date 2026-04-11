using Gantry.Services.Mediator.Filters;

namespace Gantry.Services.HarmonyPatches.Annotations;

/// <summary>
///     Denotes that the decorated object requires a specific mod to be enabled within the gameworld.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public sealed class HarmonyRequiresModAttribute : Attribute
{
    /// <summary>
    ///  	Initialises a new instance of the <see cref="RequiresModAttribute"/> class.
    /// </summary>
    /// <param name="modIds">The mod identifiers of the required mods.</param>
    public HarmonyRequiresModAttribute(params string[] modIds) => ModIds = modIds;

    /// <summary>
    ///     The ModIDs of the required mods.
    /// </summary>
    public string[] ModIds { get; }
}