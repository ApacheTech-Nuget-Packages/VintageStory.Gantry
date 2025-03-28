﻿#nullable enable
using Vintagestory.API.Server;
using Vintagestory.Server;

#pragma warning disable CS8603 // Possible null reference return.

namespace Gantry.Core;

/// <summary>
///     Provides access to the game's core API from anywhere, within a static context.
/// </summary>
/// <remarks>
///     Use api.UseGantry(options) to configure this helper before use.
/// </remarks>
[DoNotPruneType]
public static class ApiEx
{
    private static AsyncLocal<ClientMain?> _clientMain = new();
    private static AsyncLocal<ServerMain?> _serverMain = new();
    private static Thread? _serverThread;
    private static Thread? _clientThread;

    #region Initialisation

    internal static void Initialise(ICoreAPI api)
    {
        switch (api.Side)
        {
            case EnumAppSide.Server:
                _serverMain.Value = api.World as ServerMain;
                _serverThread = Thread.CurrentThread;
                G.Logger.VerboseDebug("ApiEx: Added ServerMain (Thread ID: {0}).", _serverThread.ManagedThreadId);
                break;
            case EnumAppSide.Client:
                _clientMain.Value = api.World as ClientMain;
                _clientThread = Thread.CurrentThread;
                G.Logger.VerboseDebug("ApiEx: Added ClientMain (Thread ID: {0}).", _clientThread.ManagedThreadId);
                break;
            case EnumAppSide.Universal:
            default:
                throw new ArgumentOutOfRangeException(nameof(api), api, "App-side cannot be determined.");
        }
    }

    internal static void Dispose(ICoreAPI api)
    {
        switch (api.Side)
        {
            case EnumAppSide.Server:
                _serverMain = new();
                _serverThread = null;
                break;
            case EnumAppSide.Client:
                _clientMain = new();
                _clientThread = null;
                break;
            case EnumAppSide.Universal:
            default:
                throw new ArgumentOutOfRangeException(nameof(api), api, "App-side cannot be determined.");
        }
        G.Logger.VerboseDebug($"ApiEx {api.Side} Disposed.");
    }
    #endregion

    #region API Instances

    /// <summary>
    ///     The main thread for the current app side.
    /// </summary>
    public static Thread MainThread => OneOf(_clientThread, _serverThread);

    /// <summary>
    ///     The core API implemented by the client.<br/>
    ///     The main interface for accessing the client.<br/>
    ///     Contains all subcomponents, and some miscellaneous methods.
    /// </summary>
    /// <value>The client-side API.</value>
    public static ICoreClientAPI Client => ClientMain?.Api as ICoreClientAPI;

    /// <summary>
    ///     The core API implemented by the server.<br/>
    ///     The main interface for accessing the server.<br/>
    ///     Contains all subcomponents, and some miscellaneous methods.
    /// </summary>
    public static ICoreServerAPI Server => ServerMain?.Api as ICoreServerAPI;

    /// <summary>
    ///     Common API Components that are available on the server and the client.<br/>
    ///     Cast to ICoreServerAPI, or ICoreClientAPI, to access side specific features.
    /// </summary>
    public static ICoreAPI Current => OneOf<ICoreAPI>(Client, Server);

    /// <summary>
    ///     The concrete implementation of the <see cref="IClientWorldAccessor"/> interface.<br/>
    ///     Contains access to lots of world manipulation and management features.
    /// </summary>
    /// <value>
    ///     The <see cref="Vintagestory.Client.NoObf.ClientMain"/> instance that controls access to features within the gameworld.
    /// </value>
    public static ClientMain ClientMain => _clientMain.Value;

    /// <summary>
    ///     The concrete implementation of the <see cref="IServerWorldAccessor"/> interface.
    ///     Contains access to lots of world manipulation and management features.
    /// </summary>
    /// <value>
    ///     The <see cref="Vintagestory.Server.ServerMain"/> instance that controls access to features within  the gameworld.
    /// </value>
    public static ServerMain ServerMain => _serverMain.Value;

    /// <summary>
    ///     Gets the current app-side.
    /// </summary>
    /// <value>A <see cref="EnumAppSide"/> value, representing current app-side; Client, or Server.</value>
    public static EnumAppSide Side
    {
        get
        {
            try
            {
                return ServerMain is not null 
                    ? EnumAppSide.Server 
                    : EnumAppSide.Client;
            }
            catch
            {
                return EnumAppSide.Universal;
            }
        }
    }

    #endregion

    #region Run Methods

    /// <summary>
    ///     Invokes an action, based on whether it's being called by the client, or the server.
    /// </summary>
    /// <param name="clientAction">The client action.</param>
    /// <param name="serverAction">The server action.</param>
    public static void Run(Action clientAction, Action serverAction)
    {
        OneOf(clientAction, serverAction).Invoke();
    }

    /// <summary>
    ///     Invokes an action, based on whether it's being called by the client, or the server.
    /// </summary>
    /// <remarks>
    ///     This generic method works best with the Options Pattern, rather than anonymous tuples, when passing in multiple values as a single parameter.
    /// </remarks>
    /// <param name="clientAction">The client action.</param>
    /// <param name="serverAction">The server action.</param>
    /// <param name="parameter">The parameter to pass to the invoked action.</param>
    public static void Run<T>(Action<T> clientAction, Action<T> serverAction, T parameter)
    {
        OneOf(clientAction, serverAction).Invoke(parameter);
    }

    /// <summary>
    ///     Invokes an action, based on whether it's being called by the client, or the server.
    /// </summary>
    /// <remarks>
    ///     This generic method works best with the Options Pattern, rather than anonymous tuples, when passing in multiple values as a single parameter.
    /// </remarks>
    /// <param name="clientAction">The client action.</param>
    /// <param name="serverAction">The server action.</param>
    public static void Run(Action<ICoreClientAPI> clientAction, Action<ICoreServerAPI> serverAction)
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
    public static async Task RunAsync(System.Func<ICoreClientAPI, Task> clientAction, System.Func<ICoreServerAPI, Task> serverAction)
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

    #endregion

    #region Return Methods

    /// <summary>
    ///     Invokes an action, based on whether it's being called by the client, or the server.
    /// </summary>
    /// <remarks>
    ///     This generic method works best with the Options Pattern, rather than anonymous tuples, when passing in multiple values as a single parameter.
    /// </remarks>
    /// <param name="clientAction">The client action.</param>
    /// <param name="serverAction">The server action.</param>
    public static T Return<T>(System.Func<ICoreClientAPI, T> clientAction, System.Func<ICoreServerAPI, T> serverAction)
    {
        return Side switch
        {
            EnumAppSide.Server => serverAction(Server!),
            EnumAppSide.Client => clientAction(Client!),
            EnumAppSide.Universal => throw new InvalidOperationException(
                "Cannot determine app-side. Enum evaluated to 'Universal'."),
            _ => throw new ArgumentOutOfRangeException(nameof(clientAction))
        };
    }

    /// <summary>
    ///     Invokes an action, based on whether it's being called by the client, or the server.
    /// </summary>
    /// <remarks>
    ///     This generic method works best with the Options Pattern, rather than anonymous tuples, when passing in multiple values as a single parameter.
    /// </remarks>
    /// <param name="clientAction">The client action.</param>
    /// <param name="serverAction">The server action.</param>
    public static T? Return<T>(System.Func<T> clientAction, System.Func<T> serverAction)
    {
        return (T?)OneOf(clientAction, serverAction).DynamicInvoke();
    }

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
    public static T OneOf<T>(T clientObject, T serverObject)
    {
        return Side.IsClient() ? clientObject : serverObject;
    }

    #endregion
}