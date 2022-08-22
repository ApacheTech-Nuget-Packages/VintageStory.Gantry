using HarmonyLib;
using JetBrains.Annotations;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace Gantry.Tests.AcceptanceMod.Features.HarmonyPatching.Patches
{
    /// <summary>
    ///     Harmony Patches for the <see cref="OriginalClass"/> class. This class cannot be inherited.
    /// </summary>
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public sealed class OriginalClassManualPatches
    {
        /// <summary>
        ///     Applies a <see cref="HarmonyPostfix"/> patch to the "OriginalClientMethod" method in the <see cref="OriginalClass"/> class.
        /// </summary>
        public static void Patch_OriginalClass_OriginalClientMethod_Postfix(
            ICoreClientAPI capi)
        {
            capi.ShowChatMessage("Client Postfix Patch: Injected manually.");
        }

        /// <summary>
        ///     Applies a <see cref="HarmonyPostfix"/> patch to the "OriginalServerMethod" method in the <see cref="OriginalClass"/> class.
        /// </summary>
        public static void Patch_OriginalClass_OriginalServerMethod_Postfix(
            ICoreServerAPI sapi, IServerPlayer player, int groupId)
        {
            sapi.SendMessage(player, groupId, "Server Postfix Patch: Injected manually.", EnumChatType.Notification);
        }
    }
}