namespace Gantry.Services.BrighterChat;

/// <summary>
///     Provides methods for handling chat commands using mediation.
/// </summary>
public static class MediatedChatCommands
{
    /// <summary>
    ///     Handles a chat command by creating an instance of the specified command type, sending it via the IOC container, and returning the result.
    /// </summary>
    /// <typeparam name="TCommand">The type of command to handle. It must inherit from <see cref="MediatedChatCommand"/>, and have a parameterless constructor.</typeparam>
    /// <param name="args">The arguments for the chat command.</param>
    /// <returns>The result of executing the command.</returns>
    public static TextCommandResult Handle<TCommand>(TextCommandCallingArgs args)
        where TCommand : MediatedChatCommand, new()
    {
        var command = new TCommand { Args = args };
        IOC.CommandProcessor.Send(command);
        return command.Result;
    }

    /// <summary>
    ///     Handles a chat command by creating an instance of the specified command type, sending it via the IOC container, and returning the result.
    /// </summary>
    /// <typeparam name="TCommand">The type of command to handle. It must inherit from <see cref="MediatedChatCommand"/>.</typeparam>
    /// <param name="command">The chat command.</param>
    /// <returns>The result of executing the command.</returns>
    public static TextCommandResult Handle<TCommand>(TCommand command)
        where TCommand : MediatedChatCommand
    {
        IOC.CommandProcessor.Send(command);
        return command.Result;
    }
}