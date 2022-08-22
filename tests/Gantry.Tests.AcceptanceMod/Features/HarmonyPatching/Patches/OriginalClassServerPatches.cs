using Gantry.Services.HarmonyPatches.Annotations;
using HarmonyLib;
using JetBrains.Annotations;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace Gantry.Tests.AcceptanceMod.Features.HarmonyPatching.Patches
{
    /// <summary>
    ///     Harmony Patches for the <see cref="OriginalClass"/> class. This class cannot be inherited.
    /// </summary>
    [HarmonySidedPatch(EnumAppSide.Server)]
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public sealed class OriginalClassServerPatches
    {
        /// <summary>
        ///     Applies a <see cref="HarmonyPrefix"/> patch to the "OriginalServerMethod" method in the <see cref="OriginalClass"/> class.
        /// </summary>
        [HarmonyPrefix]
        [HarmonyPatch(typeof(OriginalClass), nameof(OriginalClass.OriginalServerMethod))]
        public static bool Patch_OriginalClass_OriginalServerMethod_Prefix(
            ICoreServerAPI sapi, IServerPlayer player, int groupId)
        {
            sapi.SendMessage(player, groupId, "Server Prefix Patch: Injected automatically.", EnumChatType.Notification);
            return true;
        }
    }
}