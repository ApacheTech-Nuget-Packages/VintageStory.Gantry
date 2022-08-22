using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using ApacheTech.Common.Extensions.Harmony;
using JetBrains.Annotations;
using Vintagestory.API.Server;
using Vintagestory.Server;

// ReSharper disable StringLiteralTypo

namespace Gantry.Core.Extensions.Threading
{
    /// <summary>
    ///     Provides methods for injecting ServerSystems into the game.
    /// </summary>
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public static class ServerThreadInjectionExtensions
    {
        private static readonly Type ServerThread;

        /// <summary>
        ///     Initialises static members of the <see cref="ServerThreadInjectionExtensions" /> class.
        /// </summary>
        static ServerThreadInjectionExtensions()
        {
            ServerThread = typeof(ServerMain).Assembly.GetClassType("ServerThread");
        }

        /// <summary>
        ///     Determines whether a given ServerSystem is present within the game's registry.
        /// </summary>
        /// <param name="api">
        ///     The core API implemented by the server. The main interface for accessing the server.
        ///     Contains all sub-components, and some miscellaneous methods.
        /// </param>
        /// <typeparam name="TServerSystem">The type of the ServerSystem to find.</typeparam>
        /// <returns><c>true</c> if the ServerSystem is loaded; otherwise, <c>false</c>.</returns>
        public static bool IsServerSystemLoaded<TServerSystem>(this ICoreServerAPI api)
            where TServerSystem : ServerSystem
        {
            return api.World.GetServerSystems().Any(clientSystem => clientSystem.GetType() == typeof(TServerSystem));
        }

        /// <summary>
        ///     Gets a list of all currently running server threads.
        /// </summary>
        /// <param name="world">The world accessor API for the server.</param>
        /// <returns>A list, containing all the currently running threads, for the server process.</returns>
        public static List<Thread> GetServerThreads(this IServerWorldAccessor world)
        {
            return (world as ServerMain).GetField<List<Thread>>("Serverthreads");
        }

        /// <summary>
        ///     Retrieves all currently registered <see cref="ServerSystem" />s.
        /// </summary>
        /// <param name="world">The world accessor API for the server.</param>
        /// <returns>A <see cref="Stack{T}" />, containing all the currently registered systems, on the server.</returns>
        public static Stack<ServerSystem> GetServerSystems(this IServerWorldAccessor world)
        {
            return new Stack<ServerSystem>((world as ServerMain).GetField<ServerSystem[]>("Systems"));
        }

        /// <summary>
        ///     Injects custom thread into the server process, passing control of 
        ///     the thread's lifetime and integration, from the mod, to the game.
        /// </summary>
        /// <param name="sapi">The ServerCore API.</param>
        /// <param name="name">The name of the thread to inject.</param>
        /// <param name="systems">One or more custom <see cref="ServerSystem" /> implementations to run on the thread.</param>
        public static void InjectServerThread(this ICoreServerAPI sapi, string name, params ServerSystem[] systems)
        {
            InjectServerThread(sapi.World, name, systems);
        }

        /// <summary>
        ///     Injects custom thread into the server process, passing control of 
        ///     the thread's lifetime and integration, from the mod, to the game.
        /// </summary>
        /// <param name="world">The world accessor API for the server.</param>
        /// <param name="name">The name of the thread to inject.</param>
        /// <param name="systems">One or more custom <see cref="ServerSystem" /> implementations to run on the thread.</param>
        public static void InjectServerThread(this IServerWorldAccessor world, string name,
            params ServerSystem[] systems)
        {
            var instance = CreateServerThread(world, name, systems);
            var serverThreads = world.GetServerThreads();
            var vanillaSystems = world.GetServerSystems();

            foreach (var system in systems) vanillaSystems.Push(system);

            (world as ServerMain).SetField("Systems", vanillaSystems.ToArray());

            var thread = new Thread(() => instance.CallMethod("Process")) { IsBackground = true, Name = name };
            serverThreads.Add(thread);
        }

        private static object CreateServerThread(IServerWorldAccessor world, string name, IEnumerable<ServerSystem> systems)
        {
            var instance = ServerThread.CreateInstance();
            instance.SetField("server", world as ServerMain);
            instance.SetField("threadName", name);
            instance.SetField("serversystems", systems.ToArray());
            instance.SetField("lastFramePassedTime", new Stopwatch());
            instance.SetField("totalPassedTime", new Stopwatch());
            instance.SetField("paused", false);
            instance.SetField("alive", true);
            return instance;
        }
    }
}