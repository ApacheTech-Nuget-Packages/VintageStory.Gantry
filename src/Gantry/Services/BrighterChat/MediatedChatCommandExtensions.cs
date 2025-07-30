using ApacheTech.Common.BrighterSlim;

namespace Gantry.Services.BrighterChat;

/// <summary>
///     Extension methods to aid the use of mediated chat commands.
/// </summary>
public static class MediatedChatCommandExtensions
{
    /// <summary>
    ///     Define the mediator pipeline to be called when the command is executed.
    /// </summary>
    /// <typeparam name="TCommand"></typeparam>
    /// <param name="chatCommand"></param>
    /// <param name="commandProcessor">The command processor to use for handling the command.</param>
    public static IChatCommand WithMediatedHandler<TCommand>(this IChatCommand chatCommand, IAmACommandProcessor commandProcessor)
        where TCommand : MediatedChatCommand, new()
        => chatCommand.HandleWith(args => MediatedChatCommands.HandleCommand<TCommand>(args, commandProcessor));
}