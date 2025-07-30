using Gantry.Core.Abstractions;

namespace Gantry.Services.HarmonyPatches;

/// <summary>
///     Options for creating the Harmony Patching Service.
/// </summary>
public class HarmonyPatchingServiceOptions
{
    internal static HarmonyPatchingServiceOptions Default(ICoreGantryAPI core) => new() 
    {
        DefaultInstanceName = core.Mod.Info.ModID 
    };

    /// <summary>
    ///     Should patches from the mod assembly be automatically applied to the game? Default: True.
    /// </summary>
    public bool AutoPatchModAssembly { get; set; } = true;

    /// <summary>
    ///     The ID of the default <see cref="Harmony"/> instance used by the mod. Default: ModEx.ModAssembly.FullName.
    /// </summary>
    public required string DefaultInstanceName { get; set; }
}