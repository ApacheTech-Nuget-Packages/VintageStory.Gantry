using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using ApacheTech.Common.Extensions.Harmony;
using JetBrains.Annotations;
using Vintagestory.API.Client;
using Vintagestory.Client.NoObf;

// ReSharper disable StringLiteralTypo

namespace Gantry.Core.Extensions.Threading
{
    /// <summary>
    ///     Provides methods for injecting ClientSystems into the game.
    /// </summary>
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public static class ClientThreadInjectionExtensions
    {
        private static readonly Type ClientThread;

        /// <summary>
        /// 	Initialises static members of the <see cref="ClientThreadInjectionExtensions"/> class.
        /// </summary>
        static ClientThreadInjectionExtensions()
        {
            ClientThread = typeof(ClientMain).Assembly.GetClassType("ClientThread");
        }

        /// <summary>
        ///     Determines whether a given ClientSystem is present within the game's registry.
        /// </summary>
        /// <param name="api">
        ///     The core API implemented by the client. The main interface for accessing the client. Contains all
        ///     sub-components, and some miscellaneous methods.
        /// </param>
        /// <param name="name">The name of the ClientSystem to find.</param>
        /// <returns><c>true</c> if the ClientSystem is loaded; otherwise, <c>false</c>.</returns>
        public static bool IsClientSystemLoaded(this ICoreClientAPI api, string name)
        {
            return api.World.GetClientSystems().Any(clientSystem => clientSystem.Name == name);
        }

        /// <summary>
        ///     Determines whether a given ClientSystem is present within the game's registry.
        /// </summary>
        /// <param name="api">
        ///     The core API implemented by the client. The main interface for accessing the client. Contains all
        ///     sub-components, and some miscellaneous methods.
        /// </param>
        /// <typeparam name="TClientSystem">The type of the ClientSystem to find.</typeparam>
        /// <returns><c>true</c> if the ClientSystem is loaded; otherwise, <c>false</c>.</returns>
        public static bool IsClientSystemLoaded<TClientSystem>(this ICoreClientAPI api)
            where TClientSystem : ClientSystem
        {
            return api.World.GetClientSystems().Any(clientSystem => clientSystem.GetType() == typeof(TClientSystem));
        }

        /// <summary>
        ///     Gets a list of all currently running client threads.
        /// </summary>
        /// <param name="world">The world accessor API for the client.</param>
        /// <returns>A list, containing all the currently running threads, for the client process.</returns>
        public static List<Thread> GetClientThreads(this IClientWorldAccessor world)
        {
            return (world as ClientMain).GetField<List<Thread>>("clientThreads");
        }

        /// <summary>
        ///     Retrieves all currently registered <see cref="ClientSystem" />s.
        /// </summary>
        /// <param name="world">The world accessor API for the client.</param>
        /// <returns>A <see cref="Stack{T}" />, containing all the currently registered systems, on the client.</returns>
        public static Stack<ClientSystem> GetClientSystems(this IClientWorldAccessor world)
        {
            return new Stack<ClientSystem>((world as ClientMain).GetField<ClientSystem[]>("clientSystems"));
        }

        /// <summary>
        ///     Injects custom thread into the client process, passing control of 
        ///     the thread's lifetime and integration, from the mod, to the game.
        /// </summary>
        /// <param name="capi">The internal API for the client.</param>
        /// <param name="name">The name of the thread to inject.</param>
        /// <param name="systems">One or more custom <see cref="ClientSystem" /> implementations to run on the thread.</param>
        public static void InjectClientThread(this ICoreClientAPI capi, string name, params ClientSystem[] systems)
        {
            capi.World.InjectClientThread(name, systems);
        }

        /// <summary>
        ///     Injects custom thread into the client process, passing control of 
        ///     the thread's lifetime and integration, from the mod, to the game.
        /// </summary>
        /// <param name="world">The world accessor API for the client.</param>
        /// <param name="name">The name of the thread to inject.</param>
        /// <param name="systems">One or more custom <see cref="ClientSystem" /> implementations to run on the thread.</param>
        public static void InjectClientThread(this IClientWorldAccessor world, string name,  params ClientSystem[] systems)
        {
            var instance = CreateClientThread(world, name, systems);
            var clientThreads = world.GetClientThreads();
            var vanillaSystems = world.GetClientSystems();

            foreach (var system in systems) vanillaSystems.Push(system);

            (world as ClientMain).SetField("clientSystems", vanillaSystems.ToArray());

            var thread = new Thread(() => instance.CallMethod("Process")) { IsBackground = true };
            thread.Start();
            thread.Name = name;
            clientThreads.Add(thread);
        }

        private static object CreateClientThread(IClientWorldAccessor world, string name,
            IEnumerable<ClientSystem> systems)
        {
            var instance = ClientThread.CreateInstance();
            instance.SetField("game", world as ClientMain);
            instance.SetField("threadName", name);
            instance.SetField("clientsystems", systems.ToArray());
            instance.SetField("lastFramePassedTime", new Stopwatch());
            instance.SetField("totalPassedTime", new Stopwatch());
            instance.SetField("paused", false);
            return instance;
        }
    }
}