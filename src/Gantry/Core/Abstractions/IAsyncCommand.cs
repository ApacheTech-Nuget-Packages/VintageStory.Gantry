namespace Gantry.Core.Abstractions;

/// <summary>
///    Represents an asynchronous command that can be executed.
/// </summary>
public interface IAsyncCommand
{
    /// <summary>
    ///     Asynchronously executes the command.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    Task ExecuteAsync(CancellationToken cancellationToken = default);
}