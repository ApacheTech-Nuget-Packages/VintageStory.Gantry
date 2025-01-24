using ApacheTech.Common.BrighterSlim;
using Gantry.Core.Annotation;
using Gantry.Services.Brighter.Filters;
using Gantry.Services.Brighter.Abstractions;
using Vintagestory.API.Server;
using Vintagestory.Server;

namespace Gantry.Services.BrighterChat.Commands;

/// <summary>
///     Broadcasts a message to all players on the server.
/// </summary>
/// <seealso cref="CommandBase" />
public class BroadcastMessageToAllPlayersCommand : CommandBase
{
    /// <summary>
    ///  	Initialises a new instance of the <see cref="BroadcastMessageToAllPlayersCommand"/> class.
    /// </summary>
    /// <param name="messageCode">The i18n code for the message to send to all the players on the server.</param>
    /// <param name="localiseForEachPlayer">Determines whether the message should be localised on the server, before sending.</param>
    /// <param name="args">An options set of arguments to pass into the localised message.</param>
    public BroadcastMessageToAllPlayersCommand(string messageCode, bool localiseForEachPlayer = false, params object[] args)
    {
        MessageCode = messageCode;
        LocaliseForEachPlayer = localiseForEachPlayer;
        Arguments = args;
    }

    /// <summary>
    ///     The i18n code for the message to send to all the players on the server.
    /// </summary>
    public string MessageCode { get; }

    /// <summary>
    ///     Should the message be translated for each player individually?
    /// </summary>
    public bool LocaliseForEachPlayer { get; }

    /// <summary>
    ///     Any arguments that need to be passed into the message template.
    /// </summary>
    public object[] Arguments { get; }

    /// <summary>
    ///     Broadcasts a message to all players on the server.
    /// </summary>
    [ServerSide]
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
}