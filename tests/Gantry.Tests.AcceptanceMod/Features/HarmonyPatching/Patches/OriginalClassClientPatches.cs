using Gantry.Services.HarmonyPatches.Annotations;
using HarmonyLib;
using JetBrains.Annotations;
using Vintagestory.API.Client;
using Vintagestory.API.Common;

namespace Gantry.Tests.AcceptanceMod.Features.HarmonyPatching.Patches
{
    /// <summary>
    ///     Harmony Patches for the <see cref="OriginalClass"/> class. This class cannot be inherited.
    /// </summary>
    [HarmonySidedPatch(EnumAppSide.Client)]
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public sealed class OriginalClassClientPatches
    {
        /// <summary>
        ///     Applies a <see cref="HarmonyPrefix"/> patch to the "OriginalClientMethod" method in the <see cref="OriginalClass"/> class.
        /// </summary>
        [HarmonyPrefix]
        [HarmonyPatch(typeof(OriginalClass), nameof(OriginalClass.OriginalClientMethod))]
        public static bool Patch_OriginalClass_OriginalClientMethod_Prefix(
            ICoreClientAPI capi)
        {
            capi.ShowChatMessage("Client Prefix Patch: Injected automatically.");
            return true;
        }
    }
}