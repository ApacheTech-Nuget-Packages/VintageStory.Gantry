using ApacheTech.Common.Extensions.Harmony;

// ReSharper disable StringLiteralTypo

namespace Gantry.Core.Extensions.Threading;

/// <summary>
///     Provides methods for injecting ClientSystems into the game.
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class ClientThreadInjectionExtensions
{
    /// <summary>
    ///     Determines whether a given ClientSystem is present within the game's registry.
    /// </summary>
    /// <param name="api">
    ///     The core API implemented by the client. The main interface for accessing the client. Contains all
    ///     subcomponents, and some miscellaneous methods.
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
    ///     subcomponents, and some miscellaneous methods.
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
        => new((world as ClientMain).GetField<ClientSystem[]>("clientSystems"));

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
    public static void InjectClientThread(this IClientWorldAccessor world, string name, params ClientSystem[] systems)
    {
        var clientThreads = world.GetClientThreads();
        var thread = new Thread(() =>
        {
            var instance = CreateClientThreadInstance(world, name, systems);
            instance.CallMethod("Process");
        })
        { 
            IsBackground = true 
        };
        thread.Start();
        thread.Name = name;
        clientThreads.Add(thread);
    }

    /// <summary>
    ///     Instantiates an internal <c>ClientThread</c> object from the third-party library using Harmony's AccessTools.
    /// </summary>
    /// <param name="world">The world accessor API for the client.</param>
    /// <param name="threadName">A <c>string</c> representing the name of the thread.</param>
    /// <param name="clientSystems">An array of <c>ClientSystem</c> objects.</param>
    /// <returns>
    ///     An instance of the internal <c>ClientThread</c> object as an <c>object</c>.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when the <c>ClientThread</c> type or its constructor cannot be found.
    /// </exception>
    public static object CreateClientThreadInstance(IClientWorldAccessor world, string threadName, object[] clientSystems)
    {
        // Retrieve the internal ClientThread type by its full name.
        var clientThreadType = AccessTools.TypeByName("Vintagestory.Client.NoObf.ClientThread") 
            ?? throw new InvalidOperationException("The ClientThread type could not be found.");

        // Retrieve the constructor information for the internal class.
        var constructor = AccessTools.Constructor(clientThreadType, [typeof(ClientMain), typeof(string), typeof(ClientSystem[]), typeof(CancellationToken)]) 
            ?? throw new InvalidOperationException("The constructor for ClientThread with the specified signature could not be found.");

        // Instantiate the internal class using the obtained constructor.
        var cts = world.GetField<CancellationTokenSource>("_clientThreadsCts");
        var instance = constructor.Invoke([world, threadName, clientSystems, cts.Token]);
        return instance;
    }
}