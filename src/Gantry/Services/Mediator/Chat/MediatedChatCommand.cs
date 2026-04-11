using Gantry.Services.Mediator.Abstractions;

namespace Gantry.Services.Mediator.Chat;

/// <summary>
///     Represents a base class for chat commands that are mediated. 
///     Derived classes should implement the specific behaviour for the command.
/// </summary>
public abstract class MediatedChatCommand : GantryCommandBase
{
    /// <summary>
    ///     Gets or sets the arguments for the chat command.
    /// </summary>
    public TextCommandCallingArgs Args { get; set; } = default!;

    /// <summary>
    ///     Gets the result of executing the chat command.
    /// </summary>
    public TextCommandResult Result { get; set; } = default!;
}