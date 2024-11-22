namespace Gantry.Core.Contracts;

/// <summary>
///     Represents a command, by way of The Command Pattern.
/// </summary>
public interface IAsyncCommand
{
    /// <summary>
    ///     Executes this command.
    /// </summary>
    void ExecuteAsync();
}