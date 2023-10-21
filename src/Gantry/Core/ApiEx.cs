using System.Diagnostics;
using ApacheTech.Common.Extensions.System;
using Gantry.Core.Diagnostics;
using Gantry.Core.Extensions.Api;
using JetBrains.Annotations;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Server;
using Vintagestory.Client.NoObf;
using Vintagestory.Server;

// ReSharper disable StringLiteralTypo
// ReSharper disable CommentTypo

namespace Gantry.Core;

/// <summary>
///     Provides access to the game's core API from anywhere, within a static context.
/// </summary>
/// <remarks>
///     Use api.UseGantry(options) to configure this helper before use.
/// </remarks>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class ApiEx
{
    #region Initialisation

    internal static void Initialise(ICoreAPI api)
    {
        switch (api.Side)
        {
            case EnumAppSide.Server:
                ServerMain = Ensure.PopulatedWith(ServerMain, api.World as ServerMain);
                break;
            case EnumAppSide.Client:
                ClientMain = Ensure.PopulatedWith(ClientMain, api.World as ClientMain);
                break;
            case EnumAppSide.Universal:
            default:
                throw new ArgumentOutOfRangeException(nameof(api), api, "App-side cannot be determined.");
        }
    }

    #endregion

    #region API Instances

    /// <summary>
    ///     The core API implemented by the client.<br/>
    ///     The main interface for accessing the client.<br/>
    ///     Contains all sub-components, and some miscellaneous methods.
    /// </summary>
    /// <value>The client-side API.</value>
    public static ICoreClientAPI Client => ClientMain?.Api as ICoreClientAPI;

    /// <summary>
    ///     The core API implemented by the server.<br/>
    ///     The main interface for accessing the server.<br/>
    ///     Contains all sub-components, and some miscellaneous methods.
    /// </summary>
    public static ICoreServerAPI Server => ServerMain?.Api as ICoreServerAPI;

    /// <summary>
    ///     Common API Components that are available on the server and the client.<br/>
    ///     Cast to ICoreServerAPI, or ICoreClientAPI, to access side specific features.
    /// </summary>
    public static ICoreAPI Current => OneOf<ICoreAPI>(Client, Server);

    /// <summary>
    ///     Side-agnostic file-based logging facility.
    /// </summary>
    public static ILogger Log => Current.Logger;

    /// <summary>
    ///     The concrete implementation of the <see cref="IClientWorldAccessor"/> interface.<br/>
    ///     Contains access to lots of world manipulation and management features.
    /// </summary>
    /// <value>
    ///     The <see cref="Vintagestory.Client.NoObf.ClientMain"/> instance that controls access to features within the gameworld.
    /// </value>
    public static ClientMain ClientMain { get; private set; }

    /// <summary>
    ///     The concrete implementation of the <see cref="IServerWorldAccessor"/> interface.
    ///     Contains access to lots of world manipulation and management features.
    /// </summary>
    /// <value>
    ///     The <see cref="Vintagestory.Server.ServerMain"/> instance that controls access to features within  the gameworld.
    /// </value>
    public static ServerMain ServerMain { get; private set; }

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
    /// <param name="clientAction">The client action.</param>
    /// <param name="serverAction">The server action.</param>
    public static void Run(Action<ICoreClientAPI> clientAction, Action<ICoreServerAPI> serverAction)
    {
        switch (ModEx.ModAppSide)
        {
            case EnumAppSide.Server:
                serverAction(Server);
                break;
            case EnumAppSide.Client:
                clientAction(Client);
                break;
            case EnumAppSide.Universal:
                break;
            default:
                throw new ArgumentOutOfRangeException(
                    nameof(ModEx.ModAppSide),
                    ModEx.ModAppSide,
                    "Mod app-side cannot be determined. Have you included a ModInfoAttribute within your assembly?");
        }
    }

    /// <summary>
    ///     Invokes an action, based on whether it's being called by the client, or the server.
    /// </summary>
    /// <param name="side">The app-side to run the action on.</param>
    /// <param name="clientAction">The client action.</param>
    /// <param name="serverAction">The server action.</param>
    public static void Run(EnumAppSide side, Action<ICoreClientAPI> clientAction, Action<ICoreServerAPI> serverAction)
    {
        switch (side)
        {
            case EnumAppSide.Server:
                serverAction(Server);
                break;
            case EnumAppSide.Client:
                clientAction(Client);
                break;
            case EnumAppSide.Universal:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(side), side, "Mod app-side cannot be determined.");
        }
    }

    /// <summary>
    ///     Invokes an action, based on whether it's being called by the client, or the server.
    /// </summary>
    /// <param name="side">The app-side to run the action on.</param>
    /// <param name="universalAction">The universal action.</param>
    public static void Run(EnumAppSide side, Action<ICoreAPI> universalAction)
    {
        switch (side)
        {
            case EnumAppSide.Server:
                universalAction(Server);
                break;
            case EnumAppSide.Client:
                universalAction(Client);
                break;
            case EnumAppSide.Universal:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(side), side, "Mod app-side cannot be determined.");
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
    /// <param name="parameter">The parameter to pass to the invoked action.</param>
    public static void Run<T>(Action<T> clientAction, Action<T> serverAction, T parameter)
    {
        OneOf(clientAction, serverAction).Invoke(parameter);
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
            EnumAppSide.Server => serverAction(Server),
            EnumAppSide.Client => clientAction(Client),
            EnumAppSide.Universal => throw new InvalidOperationException("Cannot determine app-side. Enum evaluated to 'Universal'."),
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
    public static T Return<T>(System.Func<T> clientAction, System.Func<T> serverAction)
    {
        return (T)OneOf(clientAction, serverAction).DynamicInvoke();
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
    public static T Return<T>(System.Func<T> clientAction, System.Func<T> serverAction, T parameter)
    {
        return (T)OneOf(clientAction, serverAction).DynamicInvoke(parameter);
    }

    /// <summary>
    ///     Chooses between one of two objects, based on whether it's being called by the client, or the server.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="clientObject">The client object.</param>
    /// <param name="serverObject">The server object.</param>
    /// <returns>
    ///     Returns <paramref name="clientObject"/> if called from the client, or <paramref name="serverObject"/> if called from the server.
    /// </returns>
    public static T OneOf<T>(T clientObject, T serverObject)
    {
        return ModEx.ModAppSide switch
        {
            EnumAppSide.Client => clientObject,
            EnumAppSide.Server => serverObject,
            EnumAppSide.Universal => Side.IsClient() ? clientObject : serverObject,
            _ => throw new ArgumentOutOfRangeException(nameof(ModInfo.Side), ModEx.ModAppSide, "Corrupted ModInfo data.")
        };
    }

    #endregion

    #region Side Determination

    /// <summary>
    ///     Gets the current app-side.
    /// </summary>
    /// <value>An <see cref="EnumAppSide"/> value, representing current app-side; Client, or Server.</value>
    public static EnumAppSide Side
    {
        get
        {
            var thread = Thread.CurrentThread;
            if (thread.ManagedThreadId == 1)
                return Process.GetCurrentProcess().ProcessName == "VintagestoryServer"
                    ? EnumAppSide.Server
                    : EnumAppSide.Client;
            if (thread.IsThreadPoolThread) return DetermineAppSide(thread);
            try
            {
                return ThreadSideCache[thread.ManagedThreadId];
            }
            catch (KeyNotFoundException)
            {
                var side = DetermineAppSide(thread);
                return CacheAppSide(side, thread);
            }
        }
    }

    internal static Dictionary<int, EnumAppSide> ThreadSideCache { get; } = new();

    private static EnumAppSide DetermineAppSide(Thread thread)
    {
        // Obtaining the app-side, without having direct access to a specific CoreAPI.
        // NB: This is not a fool-proof. This is a drawback of using a Threaded Server, over Dedicated Server for Single-Player games.

        // 1. If modinfo.json states the mod is only for a single side, return that side. CLIENT or SERVER.
        if (ModEx.ModAppSide is not EnumAppSide.Universal)
        {
            (Server as ICoreAPI ?? Client).Logger.GantryDebug($"Stage 1: Determined App-Side for thread {thread.ManagedThreadId} ({thread.Name}), as {ModEx.ModAppSide}, because this is a single-sided mod, in ModInfo.");
            return ModEx.ModAppSide;
        }

        // 2. If the current thread name is "SingleplayerServer", we are on the SERVER. 
        // NB: A thread's name filters down through child threads, and thread-pool threads, unless manually changed.
        if (string.Equals(thread.Name, "SingleplayerServer", StringComparison.InvariantCultureIgnoreCase))
        {
            Server.Logger.GantryDebug($"Stage 2: Determined App-Side for thread {thread.ManagedThreadId} ({thread.Name}), as {EnumAppSide.Server}, because the thread name is `SingleplayerServer`");
            return EnumAppSide.Server;
        }

        // 3. If the process name is "VintagestoryServer", we are on the SERVER.
        if (Process.GetCurrentProcess().ProcessName == "VintagestoryServer")
        {
            Server.Logger.GantryDebug($"Stage 3: Determined App-Side for thread {thread.ManagedThreadId}, as {EnumAppSide.Server}, because the process name is `VintagestoryServer`.");
            return EnumAppSide.Server;
        }

        // NB: By this stage, we know that we're in a single player game, or at least on a Threaded Server; and the ServerMain member should be populated.

        // 4. If ServerMain is not populated, we're on the CLIENT.
        if (ServerMain is null)
        {
            Client.Logger.GantryDebug($"Stage 4: Determined App-Side for thread {thread.ManagedThreadId} ({thread.Name}), as {EnumAppSide.Client}, because `ServerMain` is null, within a single-player game, or Threaded Server.");
            return EnumAppSide.Client;
        }

        // 5. If the thread's ID matches one within the server's thread list, we are on the SERVER.
        if (ServerMain.Serverthreads.Any(p => thread.ManagedThreadId == p.ManagedThreadId))
        {
            Server.Logger.GantryDebug($"Stage 5: Determined App-Side for thread {thread.ManagedThreadId} ({thread.Name}), as {EnumAppSide.Server}, because it exists within the `Serverthreads` list.");
            return EnumAppSide.Server;
        }

        // 6. By this stage, we return CLIENT as a fallback; having exhausted all knowable reasons why we'd be on the Server.
        (Client as ICoreAPI ?? Server).Logger.GantryDebug($"Stage 6: Determined App-Side for thread {thread.ManagedThreadId} ({thread.Name}), as {EnumAppSide.Client}, because we know of no other reason why it would be the server.");
        return EnumAppSide.Client;
    }

    private static EnumAppSide CacheAppSide(EnumAppSide side, Thread thread)
    {
        // NB:  It's possible that this caching can lead to false positives; especially on Single-Player or LAN worlds.
        //      To limit this, I've made it so that thread pool threads don't get cached, and neither do the main threads.
        if (thread.IsThreadPoolThread) return side;
        if (ThreadSideCache.TryGetValue(thread.ManagedThreadId, out var cachedSide))
        {
            Run(side, u => u.Logger.GantryDebug($"Thread {thread.ManagedThreadId} currently cached as {cachedSide}."));
        }
        Run(side, u => u.Logger.GantryDebug($"Caching thread {thread.ManagedThreadId} as {side}."));
        ThreadSideCache.AddOrUpdate(thread.ManagedThreadId, side);
        return side;
    }

    /// <summary>
    ///     Dumps the thread side cache to the log.
    /// </summary>
    public static void DumpThreadSideCache(ILogger logger)
    {
        logger.GantryDebug("Cached Thread Sides:");
        foreach (var kvp in ThreadSideCache)
        {
            logger.GantryDebug($"\t{kvp.Key}: {kvp.Value}");
        }
    }

    #endregion
}