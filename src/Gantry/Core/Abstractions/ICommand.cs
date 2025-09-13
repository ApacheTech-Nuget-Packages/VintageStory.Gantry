namespace Gantry.Core.Abstractions;

/// <summary>
///     Represents a command that can be executed.
/// </summary>
public interface ICommand
{
    /// <summary>
    ///     Executes the command.
    /// </summary>
    void Execute();
}
