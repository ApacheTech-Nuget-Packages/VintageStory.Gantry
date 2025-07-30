using ApacheTech.Common.BrighterSlim;
using Gantry.Services.Brighter.Abstractions;

namespace Gantry.Services.Brighter.Extensions;

/// <summary>
///     Provides extensions for sending commands using the Brighter command processor.
/// </summary>
public static class CommandProcessorExtensions
{
    /// <summary>
    ///     Sends a command using the Brighter command processor, and returns the command itself.
    /// </summary>
    /// <typeparam name="T">The type of command to send. It must inherit from <see cref="CommandBase"/>.</typeparam>
    /// <param name="commandProcessor">The command processor to use for sending the command.</param>
    /// <param name="command">The command to send.</param>
    /// <returns>The command that was sent.</returns>
    public static T Handle<T>(this IAmACommandProcessor commandProcessor, T command) where T : CommandBase
    {
        commandProcessor.Send(command);
        return command;
    }
}