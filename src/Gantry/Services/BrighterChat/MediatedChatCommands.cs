using ApacheTech.Common.BrighterSlim;
using Gantry.Services.Brighter.Extensions;

namespace Gantry.Services.BrighterChat;

/// <summary>
///     Provides methods for handling chat commands using mediation.
/// </summary>
public static class MediatedChatCommands
{
    /// <summary>
    ///     Handles a chat command by creating an instance of the specified command type, sending it via the G container, and returning the result.
    /// </summary>
    /// <typeparam name="TCommand">The type of command to handle. It must inherit from <see cref="MediatedChatCommand"/>, and have a parameterless constructor.</typeparam>
    /// <param name="commandProcessor">The command processor to use for handling the command.</param>
    /// <param name="args">The arguments for the chat command.</param>
    /// <returns>The result of executing the command.</returns>
    public static TextCommandResult HandleCommand<TCommand>(TextCommandCallingArgs args, IAmACommandProcessor commandProcessor) where TCommand : MediatedChatCommand, new()
        => commandProcessor.HandleCommand(new TCommand { Args = args });

    /// <summary>
    ///     Handles a chat command by creating an instance of the specified command type, sending it via the G container, and returning the result.
    /// </summary>
    /// <typeparam name="TCommand">The type of command to handle. It must inherit from <see cref="MediatedChatCommand"/>.</typeparam>
    /// <param name="commandProcessor">The command processor to use for handling the command.</param>
    /// <param name="command">The chat command.</param>
    /// <returns>The result of executing the command.</returns>
    public static TextCommandResult HandleCommand<TCommand>(this IAmACommandProcessor commandProcessor, TCommand command) where TCommand : MediatedChatCommand 
        => commandProcessor.Handle(command).Result;
}