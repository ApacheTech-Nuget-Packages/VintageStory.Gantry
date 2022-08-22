using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace Gantry.Tests.AcceptanceMod.Features.HarmonyPatching
{
    /// <summary>
    ///     Contains methods to test the Harmony Patching Service.
    /// </summary>
    public class OriginalClass
    {
        /// <summary>
        ///     A method that's run on the client, to provide test feedback to the player.
        /// </summary>
        public void OriginalClientMethod(ICoreClientAPI capi)
        {
            capi.ShowChatMessage("Client: The original content of the method.");
        }

        /// <summary>
        ///     A method that's run on the server, to provide test feedback to the player.
        /// </summary>
        public void OriginalServerMethod(ICoreServerAPI sapi, IServerPlayer player, int groupId)
        {
            sapi.SendMessage(player, groupId, "Server: The original content of the method.", EnumChatType.Notification);
        }
    }
}