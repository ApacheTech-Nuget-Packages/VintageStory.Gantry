namespace Gantry.Services.HarmonyPatches.Annotations;

/// <summary>
///     Indicates that the decorated class or method should be applied as a patch, on both app-sides.
/// </summary>
/// <seealso cref="HarmonySidedPatchAttribute" />
/// <seealso cref="HarmonyPatch" />
public class HarmonyUniversalPatchAttribute() : HarmonySidedPatchAttribute(EnumAppSide.Universal);