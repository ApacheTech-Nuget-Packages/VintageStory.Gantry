using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using ApacheTech.Common.Extensions.System;
using JetBrains.Annotations;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Server;
using Vintagestory.Client.NoObf;
using Vintagestory.Server;

// ReSharper disable StringLiteralTypo
// ReSharper disable CommentTypo

namespace Gantry.Core
{
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
                    Server ??= api as ICoreServerAPI;
                    break;
                case EnumAppSide.Client:
                    Client ??= api as ICoreClientAPI;
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
        public static ICoreClientAPI Client { get; private set; }

        /// <summary>
        ///     The core API implemented by the server.<br/>
        ///     The main interface for accessing the server.<br/>
        ///     Contains all sub-components, and some miscellaneous methods.
        /// </summary>
        public static ICoreServerAPI Server { get; private set; }

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
        public static ClientMain ClientMain => Client.World as ClientMain;

        /// <summary>
        ///     The concrete implementation of the <see cref="IServerWorldAccessor"/> interface.
        ///     Contains access to lots of world manipulation and management features.
        /// </summary>
        /// <value>
        ///     The <see cref="Vintagestory.Server.ServerMain"/> instance that controls access to features within  the gameworld.
        /// </value>
        public static ServerMain ServerMain => Server.World as ServerMain;

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
                EnumAppSide.Universal => throw new ArgumentOutOfRangeException(),
                _ => throw new ArgumentOutOfRangeException()
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

            // 1. If modinfo.json states the mod is only for a single side, return that side.
            if (ModEx.ModAppSide is not EnumAppSide.Universal) return ModEx.ModAppSide;

            // 2. If the process name is "VintagestoryServer", we are on the server.
            if (Process.GetCurrentProcess().ProcessName == "VintagestoryServer") return EnumAppSide.Server;

            // 3. If the current thread name is "SingleplayerServer", we are on the server. 
            // NB: A thread's name filters down through child threads, and thread-pool threads, unless manually changed.
            if (string.Equals(thread.Name, "SingleplayerServer", StringComparison.InvariantCultureIgnoreCase)) return EnumAppSide.Server;

            // By this stage, we know that we're in a single player game, or at least on a Threaded Server; and the ServerMain member should be populated.
            // 4. If Server, or ServerMain are not populated, we're on the Client, or within a Client-Only context.
            if (Server is null || ServerMain is null) return EnumAppSide.Client;

            // 5. If the thread's name matches one within the server's thread list, we are on the server.
            // 6. By this stage, we return Client as a fallback; having exhausted all knowable reasons why we'd be on the Server.
            return ServerMain.Serverthreads.Any(p =>
                string.Equals(thread.Name, p.Name, StringComparison.InvariantCultureIgnoreCase))
                ? EnumAppSide.Server
                : EnumAppSide.Client;
        }

        private static EnumAppSide CacheAppSide(EnumAppSide side, Thread thread)
        {
            // NB:  It's possible that this caching can lead to false positives; especially on Single-Player or LAN worlds.
            //      To limit this, I've made it so that thread pool threads don't get cached.
            if (thread.IsThreadPoolThread) return side;
            ThreadSideCache.AddOrUpdate(thread.ManagedThreadId, side);
            return side;
        }

        #endregion
    }
}
