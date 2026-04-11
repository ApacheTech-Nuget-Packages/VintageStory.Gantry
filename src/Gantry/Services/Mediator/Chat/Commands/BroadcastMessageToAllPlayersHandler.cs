using ApacheTech.Common.Mediator.Commands.Handlers;
using Gantry.Core.Annotation;
using Gantry.Services.Mediator.Filters;

namespace Gantry.Services.Mediator.Chat.Commands;

/// <summary>
///     Broadcasts a message to all players on the server.
/// </summary>
[ServerSide]
internal class BroadcastMessageToAllPlayersHandler(ServerMain game) : CommandHandlerBase<BroadcastMessageToAllPlayersCommand>
{
    [HandledOnServer]
    public override async Task HandleAsync(BroadcastMessageToAllPlayersCommand command, CancellationToken cancellationToken)
    {
        foreach (var player in game.AllOnlinePlayers.Cast<IServerPlayer>())
        {
            var message = command.LocaliseForEachPlayer
                ? Lang.GetL(player.LanguageCode, command.MessageCode, command.Arguments)
                : Lang.Get(command.MessageCode, command.Arguments);
            game.SendMessage(player, GlobalConstants.AllChatGroups, message, EnumChatType.Notification);
        }
    }
}