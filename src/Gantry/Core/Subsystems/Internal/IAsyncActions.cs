namespace Gantry.Core.Subsystems.Internal;

/// <summary>
///     Represents a task runner, that is injected into the game's internal system store.
/// </summary>
public interface IAsyncActions : IDisposable
{
    /// <summary>
    ///     Adds an action to the task list for the VintageMods shared thread.
    /// </summary>
    /// <param name="action">The action.</param>
    void EnqueueAsyncTask(Action action);

    /// <summary>
    ///     Adds an action to the task list for the main game thread.
    /// </summary>
    /// <param name="action">The action.</param>
    void EnqueueMainThreadTask(Action action);
}