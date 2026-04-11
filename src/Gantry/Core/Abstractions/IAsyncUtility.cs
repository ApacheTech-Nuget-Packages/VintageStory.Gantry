namespace Gantry.Core.Abstractions;

/// <summary>
///     Represents an asynchronous utility that can be run.
/// </summary>
public interface IAsyncUtility
{
    /// <summary>
    ///     Runs the utility.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task RunAsync(CancellationToken cancellationToken = default);
}