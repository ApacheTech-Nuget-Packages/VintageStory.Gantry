using ApacheTech.Common.Extensions.Harmony;

// ReSharper disable StringLiteralTypo

namespace Gantry.Extensions.Threading;

/// <summary>
///     Provides methods for injecting ServerSystems into the game.
/// </summary>
public static class ServerThreadInjectionExtensions
{
    /// <summary>
    ///     Determines whether a given ServerSystem is present within the game's registry.
    /// </summary>
    /// <param name="api">
    ///     The core API implemented by the server. The main interface for accessing the server.
    ///     Contains all subcomponents, and some miscellaneous methods.
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
        return world.To<ServerMain>().GetField<List<Thread>>("Serverthreads")!;
    }

    /// <summary>
    ///     Retrieves all currently registered <see cref="ServerSystem" />s.
    /// </summary>
    /// <param name="world">The world accessor API for the server.</param>
    /// <returns>A <see cref="Stack{T}" />, containing all the currently registered systems, on the server.</returns>
    public static Stack<ServerSystem> GetServerSystems(this IServerWorldAccessor world)
    {
        return new Stack<ServerSystem>(world.To<ServerMain>().GetField<ServerSystem[]>("Systems")!);
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
        sapi.World.InjectServerThread(name, systems);
    }

    /// <summary>
    ///     Injects custom thread into the server process, passing control of 
    ///     the thread's lifetime and integration, from the mod, to the game.
    /// </summary>
    /// <param name="world">The world accessor API for the server.</param>
    /// <param name="name">The name of the thread to inject.</param>
    /// <param name="systems">One or more custom <see cref="ServerSystem" /> implementations to run on the thread.</param>
    public static void InjectServerThread(this IServerWorldAccessor world, string name, params ServerSystem[] systems)
    {
        var game = (ServerMain)world;
        var serverThread = new ServerThread(game, name, game.ServerThreadsCts.Token)
        {
            serversystems = systems
        };
        var serverThreads = world.GetServerThreads();
        var vanillaSystems = world.GetServerSystems();
        foreach (var system in systems) vanillaSystems.Push(system);
        game.SetField("Systems", vanillaSystems.ToArray());
        var thread = new Thread(serverThread.Process) { IsBackground = true, Name = name };
        serverThreads.Add(thread);
    }
}