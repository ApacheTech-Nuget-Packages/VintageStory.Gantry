namespace Gantry.Core.Helpers;

/// <summary>
///     Provides access to the game's core API from anywhere, within an instanced context.
/// </summary>
public interface IModApiContext
{
    /// <summary>
    ///     The core API implemented by the client.<br/>
    ///     The main interface for accessing the client.<br/>
    ///     Contains all subcomponents, and some miscellaneous methods.
    /// </summary>
    /// <value>The client-side API.</value>
    ICoreClientAPI Client { get; }

    /// <summary>
    ///     The concrete implementation of the <see cref="IClientWorldAccessor"/> interface.<br/>
    ///     Contains access to lots of world manipulation and management features.
    /// </summary>
    /// <value>
    ///     The <see cref="Vintagestory.Client.NoObf.ClientMain"/> instance that controls access to features within the gameworld.
    /// </value>
    ClientMain ClientMain { get; }

    /// <summary>
    ///     Common API Components that are available on the server and the client.<br/>
    ///     Cast to ICoreServerAPI, or ICoreClientAPI, to access side specific features.
    /// </summary>
    ICoreAPI Current { get; }

    /// <summary>
    ///     The main thread for the current app side.
    /// </summary>
    Thread MainThread { get; }

    /// <summary>
    ///     The core API implemented by the server.<br/>
    ///     The main interface for accessing the server.<br/>
    ///     Contains all subcomponents, and some miscellaneous methods.
    /// </summary>
    ICoreServerAPI Server { get; }

    /// <summary>
    ///     The concrete implementation of the <see cref="IServerWorldAccessor"/> interface.
    ///     Contains access to lots of world manipulation and management features.
    /// </summary>
    /// <value>
    ///     The <see cref="Vintagestory.Server.ServerMain"/> instance that controls access to features within  the gameworld.
    /// </value>
    ServerMain ServerMain { get; }

    /// <summary>
    ///     Gets the current app-side.
    /// </summary>
    /// <value>A <see cref="EnumAppSide"/> value, representing current app-side; Client, or Server.</value>
    EnumAppSide Side { get; }

    /// <summary>
    ///     Chooses between one of two objects, based on whether it's being called by the client, or the server.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="clientObject">The client object.</param>
    /// <param name="serverObject">The server object.</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <returns>
    ///     Returns <paramref name="clientObject"/> if called from the client, or <paramref name="serverObject"/> if called from the server.
    /// </returns>
    T OneOf<T>(T clientObject, T serverObject)
        => Side.IsClient() ? clientObject : serverObject;

    /// <summary>
    ///     Invokes an action, based on whether it's being called by the client, or the server.
    /// </summary>
    /// <remarks>
    ///     This generic method works best with the Options Pattern, rather than anonymous tuples, when passing in multiple values as a single parameter.
    /// </remarks>
    /// <param name="clientAction">The client action.</param>
    /// <param name="serverAction">The server action.</param>
    T Return<T>(System.Func<ICoreClientAPI, T> clientAction, System.Func<ICoreServerAPI, T> serverAction)
        => Side switch
        {
            EnumAppSide.Server => serverAction(Server!),
            EnumAppSide.Client => clientAction(Client!),
            EnumAppSide.Universal => throw new InvalidOperationException(
                "Cannot determine app-side. Enum evaluated to 'Universal'."),
            _ => throw new ArgumentOutOfRangeException(nameof(clientAction))
        };

    /// <summary>
    ///     Invokes an action, based on whether it's being called by the client, or the server.
    /// </summary>
    /// <remarks>
    ///     This generic method works best with the Options Pattern, rather than anonymous tuples, when passing in multiple values as a single parameter.
    /// </remarks>
    /// <param name="clientAction">The client action.</param>
    /// <param name="serverAction">The server action.</param>
    T? Return<T>(Func<T> clientAction, Func<T> serverAction)
        => (T?)OneOf(clientAction, serverAction).DynamicInvoke();

    /// <summary>
    ///     Invokes an action, based on whether it's being called by the client, or the server.
    /// </summary>
    /// <param name="clientAction">The client action.</param>
    /// <param name="serverAction">The server action.</param>
    void Run(Action clientAction, Action serverAction)
        => OneOf(clientAction, serverAction).Invoke();

    /// <summary>
    ///     Invokes an action, based on whether it's being called by the client, or the server.
    /// </summary>
    /// <remarks>
    ///     This generic method works best with the Options Pattern, rather than anonymous tuples, when passing in multiple values as a single parameter.
    /// </remarks>
    /// <param name="clientAction">The client action.</param>
    /// <param name="serverAction">The server action.</param>
    /// <param name="parameter">The parameter to pass to the invoked action.</param>
    void Run<T>(Action<T> clientAction, Action<T> serverAction, T parameter)
        => OneOf(clientAction, serverAction).Invoke(parameter);

    /// <summary>
    ///     Invokes an action, based on whether it's being called by the client, or the server.
    /// </summary>
    /// <remarks>
    ///     This generic method works best with the Options Pattern, rather than anonymous tuples, when passing in multiple values as a single parameter.
    /// </remarks>
    /// <param name="clientAction">The client action.</param>
    /// <param name="serverAction">The server action.</param>
    void Run(Action<ICoreClientAPI> clientAction, Action<ICoreServerAPI> serverAction)
    {
        switch (Side)
        {
            case EnumAppSide.Server:
                serverAction(Server!);
                break;
            case EnumAppSide.Client:
                clientAction(Client!);
                break;
            case EnumAppSide.Universal:
                throw new InvalidOperationException("Cannot determine app-side. Enum evaluated to 'Universal'.");
            default:
                throw new ArgumentOutOfRangeException(nameof(clientAction));
        }
    }

    /// <summary>
    ///     Invokes an action, based on whether it's being called by the client, or the server.
    /// </summary>
    /// <remarks>
    ///     This generic method works best with the Options Pattern, rather than anonymous tuples, when passing in multiple values as a single parameter.
    /// </remarks>
    /// <param name="clientAction">The client action.</param>
    /// <param name="serverAction">The server action.</param>
    async Task RunAsync(System.Func<ICoreClientAPI, Task> clientAction, System.Func<ICoreServerAPI, Task> serverAction)
    {
        switch (Side)
        {
            case EnumAppSide.Server:
                await serverAction(Server!);
                break;
            case EnumAppSide.Client:
                await clientAction(Client!);
                break;
            case EnumAppSide.Universal:
                throw new InvalidOperationException("Cannot determine app-side. Enum evaluated to 'Universal'.");
            default:
                throw new ArgumentOutOfRangeException(nameof(clientAction));
        }
    }
}