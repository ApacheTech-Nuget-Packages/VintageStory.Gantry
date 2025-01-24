namespace Gantry.Core.GameContent.ChatCommands;

/// <summary>
///     Provides extension methods for handling chat commands with custom logic.
/// </summary>
public static class ChatCommandExtensions
{
    /// <summary>
    ///     Specifies a handler for the chat command, executing the handler and returning a success result.
    /// </summary>
    /// <param name="command">The chat command to associate the handler with.</param>
    /// <param name="handler">The handler to execute when the command is invoked.</param>
    /// <returns>The modified chat command.</returns>
    public static IChatCommand HandleWith(this IChatCommand command, Action<TextCommandCallingArgs> handler)
    {
        command.HandleWith(args =>
        {
            handler(args);
            return TextCommandResult.Success();
        });
        return command;
    }

    /// <summary>
    ///     Specifies a handler for the chat command, executing the handler and returning a success result.
    /// </summary>
    /// <param name="command">The chat command to associate the handler with.</param>
    /// <returns>The modified chat command.</returns>
    public static IChatCommand WithDefaultHandler(this IChatCommand command) 
        => command.HandleWith(_ => TextCommandResult.Success());

    /// <summary>
    ///     Retrieves the parsed value from the command argument parser and converts it to the specified type.
    /// </summary>
    /// <typeparam name="TValue">The target type to convert the value to.</typeparam>
    /// <param name="parser">The command argument parser instance.</param>
    /// <returns>The parsed value converted to the specified type.</returns>
    public static TValue Value<TValue>(this ICommandArgumentParser parser)
        => parser.GetValue().To<TValue>();

}