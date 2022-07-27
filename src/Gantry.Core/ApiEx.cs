using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Server;
using Vintagestory.Client.NoObf;
using Vintagestory.Server;

// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedType.Global
// ReSharper disable StringLiteralTypo
// ReSharper disable CommentTypo

namespace Gantry.Core
{
    /// <summary>
    ///     
    /// </summary>
    public static class ApiEx
    {
        private static readonly FieldInfo ServerThreadsFieldInfo =
            typeof(ServerMain).GetField("Serverthreads", BindingFlags.Instance | BindingFlags.NonPublic);
        
        /// <summary>
        ///     Initialises the API Extension Helper.
        /// </summary>
        /// <param name="api">The API.</param>
        /// <param name="assemblyMarker">The assembly marker.</param>
        /// <exception cref="TypeLoadException"></exception>
        public static void Initialise(ICoreAPI api, Type assemblyMarker)
        {
            var modInfo = assemblyMarker.Assembly
                .GetCustomAttributes(false)
                .OfType<ModInfoAttribute>()
                .FirstOrDefault();

            if (modInfo is null)
            {
                throw new TypeLoadException("Cannot extract `ModInfoAttribute` information from the marked assembly.");
            }

            Initialise(api, modInfo);
        }

        /// <summary>
        ///     Initialises the API Extension Helper.
        /// </summary>
        /// <param name="api">The API.</param>
        /// <param name="modInfo">The assembly marker.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void Initialise(ICoreAPI api, ModInfoAttribute modInfo)
        {
            ModInfo = modInfo;
            switch (api.Side)
            {
                case EnumAppSide.Server:
                    Server = api as ICoreServerAPI;
                    break;
                case EnumAppSide.Client:
                    Client = api as ICoreClientAPI;
                    break;
                case EnumAppSide.Universal:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        ///     Gets the mod information.
        /// </summary>
        /// <value>The mod information.</value>
        public static ModInfoAttribute ModInfo { get; private set; }

        /// <summary>
        ///     Gets the side designated within the mod information.
        /// </summary>
        /// <value>
        ///     The side designated within the mod information.
        /// </value>
        /// <exception cref="InvalidOperationException">Cannot determine app-side before `ApiEx` is intialised.</exception>
        private static EnumAppSide ModInfoSide
        {
            get
            {
                if (ModInfo is null)
                {
                    throw new InvalidOperationException("Cannot determine app-side before `ApiEx` is intialised.");
                }
                return (EnumAppSide)Enum.Parse(typeof(EnumAppSide), ModInfo.Side);
            }
        }

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
        /// <value>The server-side API.</value>
        public static ICoreServerAPI Server { get; private set; }

        /// <summary>
        ///     Common API Components that are available on the server and the client.<br/>
        ///     Cast to ICoreServerAPI, or ICoreClientAPI, to access side specific features.
        /// </summary>
        /// <value>The universal API.</value>
        public static ICoreAPI Current => OneOf<ICoreAPI>(Client, Server);

        /// <summary>
        ///     The concrete implementation of the <see cref="IClientWorldAccessor"/> interface.<br/>
        ///     Contains access to lots of world manipulation and management features.
        /// </summary>
        /// <value>
        ///     The <see cref="Vintagestory.Client.NoObf.ClientMain"/> instance that controls access to features within the gameworld.
        /// </value>
        public static ClientMain ClientMain => Client.World as ClientMain;

        /// <summary>
        ///     The concrete implementation of the <see cref="IServerWorldAccessor"/> interface. Contains access to lots of world manipulation and management features.
        /// </summary>
        /// <value>
        ///     The <see cref="Vintagestory.Server.ServerMain"/> instance that controls access to features within  the gameworld.
        /// </value>
        public static ServerMain ServerMain => Server.World as ServerMain;

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
            return ModInfoSide switch
            {
                EnumAppSide.Client => clientObject,
                EnumAppSide.Server => serverObject,
                EnumAppSide.Universal => Side.IsClient() ? clientObject : serverObject,
                _ => throw new ArgumentOutOfRangeException(nameof(ModInfo.Side), ModInfo.Side, "Corrupted ModInfo data.")
            };
        }

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
        ///     Determines whether a given mod is installed, and enabled, on the current app-side.
        /// </summary>
        /// <param name="modId">The mod identifier.</param>
        /// <returns><c>true</c> if the mod is enabled; otherwise, <c>false</c>.</returns>
        public static bool IsModEnabled(string modId)
        {
            return Current.ModLoader.IsModEnabled(modId);
        }

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

        /// <summary>
        ///     Determines whether the current code block is running on the main thread. See remarks.
        /// </summary>
        /// <remarks>
        ///     Within a Single-Player game, the server will never run on the main application thread.
        ///     Single-Player servers are run as a background thread, within the client application.
        /// </remarks>
        /// <returns><c>true</c> if the code is currently running on the main application thread; otherwise <c>false</c>.</returns>
        public static bool IsOnMainThread()
        {
            var thread = Thread.CurrentThread;
            return thread.GetApartmentState() == ApartmentState.STA
                   && !thread.IsBackground
                   && !thread.IsThreadPoolThread
                   && thread.IsAlive;
        }

        internal static Dictionary<int, EnumAppSide> ThreadSideCache { get; } = new();

        private static EnumAppSide DetermineAppSide(Thread thread)
        {
            // Obtaining the app-side, without having direct access to a specific CoreAPI.
            // NB: This is not a fool-proof. This is a drawback of using a Threaded Server, over Dedicated Server for Single-Player games.

            // 1. If modinfo.json states the mod is only for a single side, return that side.
            if (ModInfoSide is not EnumAppSide.Universal) return ModInfoSide;

            // 2. If the process name is "VintagestoryServer", we are on the server.
            if (Process.GetCurrentProcess().ProcessName == "VintagestoryServer") return EnumAppSide.Server;

            // 3. If the current thread name is "SingleplayerServer", we are on the server. 
            // NB: A thread's name filters down through child threads, and thread-pool threads, unless manually changed.
            if (string.Equals(thread.Name, "SingleplayerServer", StringComparison.InvariantCultureIgnoreCase)) return EnumAppSide.Server;

            // By this stage, we know that we're in a single player game, or at least on a Threaded Server; and the ServerMain member should be populated.
            // 4. If ServerMain is populated, and the thread's name matches one within the server's thread list, we are on the server.
            // 5. By this stage, we return Client as a fallback; having exhausted all knowable reasons why we'd be on the Server.
            return ServerMain?.GetServerThreads().Any(p =>
                string.Equals(thread.Name, p.Name, StringComparison.InvariantCultureIgnoreCase)) ?? false
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

        private static IEnumerable<Thread> GetServerThreads(this IServerWorldAccessor world)
        {
            var threads = (List<Thread>)ServerThreadsFieldInfo.GetValue(world);
            return threads ?? new List<Thread>();
        }

        private static void AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> collection, TKey key, TValue value)
        {
            if (!collection.ContainsKey(key))
            {
                collection.Add(key, value);
                return;
            }
            collection[key] = value;
        }
    }
}
