using Gantry.Core;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;

namespace Gantry.Tests.Integration
{
    /// <summary>
    ///     Sample ModSystem.
    /// </summary>
    /// <seealso cref="ModSystem" />
    internal class Program : ModSystem
    {
        private ICoreClientAPI _capi;

        public override void StartPre(ICoreAPI api)
        {
            ApiEx.Initialise(api, GetType());
        }

        /// <summary>
        ///     Returns if this mod should be loaded for the given app side.
        /// </summary>
        /// <param name="side">The current app-side, loading the mod.</param>
        public override bool ShouldLoad(EnumAppSide side)
        {
            return side.IsClient();
        }

        /// <summary>
        ///     Full start to the mod on the client side.
        /// 
        ///     Note, in multi-player games, the server assets (blocks, items, entities, recipes) have not yet been received
        ///     and so no blocks etc.are yet registered.
        /// 
        ///     For code that must run only after we have blocks, items, entities, and recipes all registered and loaded,
        ///     add your method to the API event, BlockTexturesLoaded.
        /// </summary>
        /// <param name="capi">The core API for the Client side of the game.</param>
        public override void StartClientSide(ICoreClientAPI capi)
        {
            _capi = (ICoreClientAPI)ApiEx.Current;
            _capi.RegisterCommand("hello", Lang.Get("gantryintegrationtests:hello-world-description"), string.Empty, OnChatCommand);
        }

        /// <summary>
        ///     Called when a user types `.hello` in chat.
        /// </summary>
        /// <param name="groupId">The chat group the player belongs to.</param>
        /// <param name="args">The arguments passed to the command.</param>
        private void OnChatCommand(int groupId, CmdArgs args)
        {
            _capi.ShowChatMessage(Lang.Get("gantryintegrationtests:hello-world-text"));
        }
    }
}
