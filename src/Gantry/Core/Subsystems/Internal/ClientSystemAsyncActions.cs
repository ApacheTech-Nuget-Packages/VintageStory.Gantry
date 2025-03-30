using System.Collections.Concurrent;

namespace Gantry.Core.Subsystems.Internal;

/// <summary>
///     An internal system that is injected into the game, on the client.
/// </summary>
/// <seealso cref="ClientSystem" />
public class ClientSystemAsyncActions : ClientSystem, IAsyncActions
{
    private readonly ClientMain _game;

    /// <summary>
    ///     Initialises a new instance of the <see cref="ClientSystemAsyncActions" /> class.
    /// </summary>
    /// <param name="game">The game.</param>
    public ClientSystemAsyncActions(ClientMain game) : base(game)
    {
        _game = game;
    }

    private ConcurrentQueue<Action> AsyncActions { get; set; } = new();

    private ConcurrentQueue<Action> MainThreadActions { get; set; } = new();

    /// <inheritdoc />
    public override string Name => "Gantry AsyncActions";

    /// <summary>
    ///     Gets the type of the system. The type determines load order, grouping, and priority.
    /// </summary>
    /// <returns>
    ///     The <see cref="EnumClientSystemType" /> type of the system, which determines load order, grouping, and
    ///     priority.
    /// </returns>
    public override EnumClientSystemType GetSystemType()
    {
        return EnumClientSystemType.Misc;
    }

    /// <summary>
    ///     Called every tick, on the thread that this system resides on.
    /// </summary>
    /// <param name="dt">The time between this tick, and the previous tick.</param>
    public override void OnSeperateThreadGameTick(float dt)
    {
        ProcessActions(AsyncActions);
        ProcessMainThreadActions();
    }

    /// <inheritdoc cref="ClientSystem.Dispose" />
    public void Dispose(ICoreClientAPI capi)
    {
        Dispose((ClientMain)capi.World);
    }

    /// <inheritdoc cref="ClientSystem.Dispose" />
    public void Dispose()
    {
        GC.SuppressFinalize(this);
        Dispose(_game);
    }

    /// <inheritdoc cref="ClientSystem.Dispose(ClientMain)" />
    public override void Dispose(ClientMain game)
    {
        if (!AsyncActions.IsEmpty)
            AsyncActions = new ConcurrentQueue<Action>();

        if (!MainThreadActions.IsEmpty)
            MainThreadActions = new ConcurrentQueue<Action>();

        base.Dispose(game);
    }

    /// <summary>
    ///     Adds an action to the task list for the VintageMods shared thread.
    /// </summary>
    /// <param name="action">The action.</param>
    public void EnqueueAsyncTask(Action action)
    {
        AsyncActions.Enqueue(action);
    }

    /// <summary>
    ///     Adds an action to the task list for the main game thread.
    /// </summary>
    /// <param name="action">The action.</param>
    public void EnqueueMainThreadTask(Action action)
    {
        MainThreadActions.Enqueue(action);
    }

    private void ProcessMainThreadActions()
    {
        if (!MainThreadActions.IsEmpty)
            _game.EnqueueMainThreadTask(() => ProcessActions(MainThreadActions), "");
    }

    private static void ProcessActions(ConcurrentQueue<Action> actions)
    {
        for (var i = 0; i < actions.Count; i++)
        {
            var success = actions.TryDequeue(out var action);
            if (success) action?.Invoke();
        }
    }
}