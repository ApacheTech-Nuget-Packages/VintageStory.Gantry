using Gantry.Core.Extensions.Api;
using JetBrains.Annotations;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Server;
using Vintagestory.Client.NoObf;

namespace Gantry.Core.Extensions
{
    /// <summary>
    ///     Extensions methods to aid working with players.
    /// </summary>
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public static class PlayerExtensions
    {
        /// <summary>
        ///     Sends a chat message to the player, using the current chat channel.
        /// </summary>
        public static void SendMessage(this IServerPlayer player, string message)
        {
            player.SendMessage(GlobalConstants.CurrentChatGroup, message, EnumChatType.Notification);
        }

        /// <summary>
        ///     Sends a chat message to the player, using the current chat channel.
        /// </summary>
        public static void ShowChatMessage(this IPlayer player, string message)
        {
            ApiEx.Side.RunOneOf(
                () => ((IClientPlayer)player).ShowChatNotification(message), 
                () => ((IServerPlayer)player).SendMessage(message));
        }

        /// <summary>
        ///     Thread-Safe.
        ///     Shows a client side only chat message in the current chat channel. Does not execute client commands.
        /// </summary>
        /// <param name="api">The core game API this method was called from.</param>
        /// <param name="message">The message to show to the player.</param>
        public static void EnqueueShowChatMessage(this ICoreClientAPI api, string message)
        {
            (api.World as ClientMain)?.EnqueueShowChatMessage(message);
        }

        /// <summary>
        ///     Thread-Safe.
        ///     Shows a client side only chat message in the current chat channel. Does not execute client commands.
        /// </summary>
        /// <param name="game">The core game API this method was called from.</param>
        /// <param name="message">The message to show to the player.</param>
        public static void EnqueueShowChatMessage(this ClientMain game, string message)
        {
            game?.EnqueueMainThreadTask(() => game.ShowChatMessage(message), "");
        }

    }
}
