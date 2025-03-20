using ApacheTech.Common.BrighterSlim;
using Gantry.Core.Annotation;
using Gantry.Services.Brighter.Filters;
using SmartAssembly.Attributes;
using Vintagestory.API.Server;
using Vintagestory.Server;

namespace Gantry.Services.BrighterChat.Commands;

/// <summary>
///     Broadcasts a message to all players on the server.
/// </summary>
[ServerSide]
[DoNotPruneType]
internal class BroadcastMessageToAllPlayersHandler(ServerMain game) : RequestHandler<BroadcastMessageToAllPlayersCommand>
{
    [HandledOnServer]
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