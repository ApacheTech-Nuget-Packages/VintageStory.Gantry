using Gantry.Core;
using HarmonyLib;
using JetBrains.Annotations;

namespace Gantry.Services.HarmonyPatches
{
    /// <summary>
    ///     Options for creating the Harmony Patching Service.
    /// </summary>
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class HarmonyPatchingServiceOptions
    {
        internal static HarmonyPatchingServiceOptions Default = new();

        /// <summary>
        ///     Should patches from the mod assembly be automatically applied to the game? Default: True.
        /// </summary>
        public bool AutoPatchModAssembly { get; set; } = true;

        /// <summary>
        ///     The ID of the default <see cref="Harmony"/> instance used by the mod. Default: ModEx.ModAssembly.FullName.
        /// </summary>
        public string DefaultInstanceName { get; set; } = ModEx.ModAssembly.FullName;
    }
}