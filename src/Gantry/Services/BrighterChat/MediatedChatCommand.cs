﻿using Gantry.Core.Brighter.Abstractions;
using Vintagestory.API.Common;

namespace Gantry.Services.BrighterChat;

/// <summary>
///     Represents a base class for chat commands that are mediated. 
///     Derived classes should implement the specific behaviour for the command.
/// </summary>
public abstract class MediatedChatCommand : CommandBase
{
    /// <summary>
    ///     Gets or sets the arguments for the chat command.
    /// </summary>
    public TextCommandCallingArgs Args { get; set; }

    /// <summary>
    ///     Gets the result of executing the chat command.
    /// </summary>
    public TextCommandResult Result { get; protected internal set; }
}