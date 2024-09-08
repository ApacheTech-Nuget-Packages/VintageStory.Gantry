using ApacheTech.Common.BrighterSlim;
using Gantry.Core.Annotation;
using Gantry.Core.Brighter.Filters;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Server;
using Vintagestory.Server;

namespace Gantry.Core.Brighter.Common;

/// <summary>
///     Broadcasts a message to all players on the server.
/// </summary>
[ServerSide]
internal class BroadcastMessageToAllPlayersHandler(ServerMain game) : RequestHandler<BroadcastMessageToAllPlayersCommand>
{
    [Side(EnumAppSide.Server, asynchronous: false)]
    public override BroadcastMessageToAllPlayersCommand Handle(BroadcastMessageToAllPlayersCommand command)
    {
        foreach (var player in game.AllOnlinePlayers.Cast<IServerPlayer>())
        {
            var message = command.LocaliseForEachPlayer
                ? Lang.GetL(player.LanguageCode, command.MessageCode, command.Arguments)
                : Lang.Get(command.MessageCode, command.Arguments);
            game.SendMessage(player, GlobalConstants.AllChatGroups, message, EnumChatType.Notification);
        }
        return base.Handle(command);
    }
}