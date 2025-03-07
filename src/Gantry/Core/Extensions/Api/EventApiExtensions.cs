namespace Gantry.Core.Extensions.Api;

// ReSharper disable once AccessToDisposedClosure

/// <summary>
///     Provides extension methods for the <see cref="IEventAPI" /> interface.
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class EventApiExtensions
{

    /// <remarks>
    ///     This method synchronises execution with the main thread by blocking the calling thread 
    ///     until the action completes. Use with care to avoid potential deadlocks or performance issues.
    /// </remarks>
    /// <summary>
    ///     Executes the specified action on the main thread, blocking the calling thread until completion.
    /// </summary>
    /// <param name="eventApi">
    ///     The event API instance used to enqueue the action on the main thread.
    /// </param>
    /// <param name="action">
    ///     The action to execute on the main thread.
    /// </param>
    /// <remarks>
    ///     This method synchronises execution with the main thread by blocking the calling thread 
    ///     until the action completes. Use with care to avoid potential deadlocks or performance issues.
    /// </remarks>
    public static void AwaitMainThreadTask(this IEventAPI eventApi, Action action)
    {
        // Check if already on the main thread, and execute inline if so.
        if (Environment.CurrentManagedThreadId == ApiEx.MainThread.ManagedThreadId)
        {
            action();
            return;
        }

        using var waitHandle = new ManualResetEventSlim(false);
        eventApi.EnqueueMainThreadTask(() =>
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                ApiEx.Logger.Error("Error in main thread task: " + ex);
                throw;
            }
            finally
            {
                ApiEx.Logger.Trace($"[{Environment.CurrentManagedThreadId}] Setting waitHandle.");
                waitHandle.Set();
            }
        }, "");
        if (!waitHandle.Wait(TimeSpan.FromSeconds(30))) // Add a timeout for safety
        {
            ApiEx.Logger.Error("Main thread task timed out.");
            throw new TimeoutException("Main thread task timed out.");
        }
    }


    /// <summary>
    ///     Executes the specified action on a background thread, blocking the calling thread until completion.
    /// </summary>
    /// <param name="eventApi">
    ///     The event API instance used to manage event-related tasks.
    /// </param>
    /// <param name="action">
    ///     The action to execute on a background thread.
    /// </param>
    /// <remarks>
    ///     This method offloads the specified action to a separate thread while synchronising its completion 
    ///     with the calling thread. Use with care to avoid performance bottlenecks or unintended blocking behaviour.
    /// </remarks>
    public static void AwaitOffThreadTask(this IEventAPI eventApi, Action action)
    {
        using var waitHandle = new ManualResetEventSlim(false);
        Task.Factory.StartNew(() =>
        {
            action();
            waitHandle.Set();
        });
        waitHandle.Wait();
    }
}