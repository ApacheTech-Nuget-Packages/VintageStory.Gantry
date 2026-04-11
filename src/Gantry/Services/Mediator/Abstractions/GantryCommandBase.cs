namespace Gantry.Services.Mediator.Abstractions;

/// <summary>
///     Acts as a base class for all Brighter commands.
/// </summary>
public abstract class GantryCommandBase : CommandBase
{
    /// <summary>
    ///     Determines whether the execution of the command was successful, or not.
    /// </summary>
    public bool Success { get; set; } = false;

    /// <summary>
    ///     If the command was not executed successfully, this list may give reasons as to why.
    /// </summary>
    public List<string> ErrorMessages { get; set; } = [];
}