namespace Gantry.Services.HarmonyPatches.Annotations;

/// <summary>
///     Indicates that the decorated class or method should be applied as a patch, but only for the server.
/// </summary>
/// <seealso cref="HarmonySidedPatchAttribute" />
/// <seealso cref="HarmonyPatch" />
public class HarmonyServerSidePatchAttribute() : HarmonySidedPatchAttribute(EnumAppSide.Server);