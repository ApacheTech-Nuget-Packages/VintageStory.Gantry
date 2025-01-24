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
    /// <returns></returns>
    public static IChatCommand WithMediatedHandler<TCommand>(this IChatCommand chatCommand)
        where TCommand : MediatedChatCommand, new()
        => chatCommand.HandleWith(MediatedChatCommands.Handle<TCommand>);
}