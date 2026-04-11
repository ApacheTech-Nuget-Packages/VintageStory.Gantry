using ApacheTech.Common.Mediator.Commands.Processor;
using Gantry.Services.Mediator.Abstractions;

namespace Gantry.Services.Mediator.Extensions;

/// <summary>
///     Provides extensions for sending commands using the Brighter command processor.
/// </summary>
public static class CommandProcessorExtensions
{
    /// <summary>
    ///     Sends a command using the Brighter command processor, and returns the command itself.
    /// </summary>
    /// <typeparam name="T">The type of command to send. It must inherit from <see cref="GantryCommandBase"/>.</typeparam>
    /// <param name="commandProcessor">The command processor to use for sending the command.</param>
    /// <param name="command">The command to send.</param>
    /// <returns>The command that was sent.</returns>
    public static T Handle<T>(this ICommandProcessor commandProcessor, T command) where T : GantryCommandBase
    {
        commandProcessor.Execute(command);
        return command;
    }
}