using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using ApacheTech.Common.Extensions.Harmony;
using JetBrains.Annotations;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.Client;
using Vintagestory.Client.NoObf;
using Vintagestory.Server;

namespace Gantry.Core.Extensions.Api;

/// <summary>
///     Extension methods for converting apis to concrete implementations.
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class ApiExtensions
{
    #region Client

    /// <summary>
    ///     Returns the <see cref="ClientMain"/> from the current <see cref="ICoreClientAPI"/> instance.
    /// </summary>
    public static ClientMain AsClientMain(this ICoreClientAPI api)
    {
        return (ClientMain)api.World;
    }

    /// <summary>
    ///     Returns the <see cref="ICoreClientAPI"/> from the current <see cref="ClientMain"/> instance.
    /// </summary>
    public static ICoreClientAPI AsApi(this ClientMain game)
    {
        return (ICoreClientAPI)game.Api;
    }

    /// <summary>
    ///     Returns the dimensions of the physical viewport the game is currently running on.
    /// </summary>
    [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Extension Method")]
    public static Vec2f ClientWindowSize(this ICoreClientAPI capi)
    {
        return new Vec2f(
            ScreenManager.Platform.WindowSize.Width,
            ScreenManager.Platform.WindowSize.Height);
    }

    /// <summary>
    ///     Unregisters an internal client system from the game engine.
    /// </summary>
    /// <typeparam name="T">The type of <see cref="ClientSystem"/> to unregister.</typeparam>
    /// <param name="capi">Core Client API.</param>
    public static void UnregisterInternalClientSystem<T>(this ICoreClientAPI capi)
        where T : ClientSystem
    {
        var clientMain = capi.World as ClientMain;
        var clientSystems = clientMain.GetField<ClientSystem[]>("clientSystems").ToList();
        for (var i = 0; i < clientSystems.Count; i++)
        {
            if (clientSystems[i].GetType() != typeof(T)) continue;
            clientSystems[i].Dispose(clientMain);
            clientSystems.Remove(clientSystems[i]);
            break;
        }

        clientMain.SetField("clientSystems", clientSystems.ToArray());
    }

    /// <summary>
    ///     Unregisters an internal client system from the game engine.
    /// </summary>
    /// <param name="capi">Core Client API.</param>
    /// <param name="name">The friendly name of the system to unregister.</param>
    public static void UnregisterInternalClientSystem(this ICoreClientAPI capi, string name)
    {
        var clientMain = capi.World as ClientMain;
        var clientSystems = clientMain.GetField<ClientSystem[]>("clientSystems").ToList();
        for (var i = 0; i < clientSystems.Count; i++)
        {
            if (clientSystems[i].Name != name) continue;
            clientSystems[i].Dispose(clientMain);
            clientSystems.Remove(clientSystems[i]);
            break;
        }

        clientMain.SetField("clientSystems", clientSystems.ToArray());
    }

    /// <summary>
    ///     Returns a specific <see cref="ClientSystem"/> that is registered with the game engine.
    /// </summary>
    /// <param name="capi">Core Client API.</param>
    /// <param name="commandName">The friendly name of the system to return.</param>
    public static object GetInternalClientSystem(this ICoreClientAPI capi, string commandName)
    {
        var clientSystems = (capi.World as ClientMain).GetField<ClientSystem[]>("clientSystems");
        return clientSystems.FirstOrDefault(p => p.Name == commandName);
    }

    /// <summary>
    ///     Returns a specific <see cref="ClientSystem"/> that is registered with the game engine.
    /// </summary>
    /// <param name="capi">Core Client API.</param>
    public static T GetInternalClientSystem<T>(this ICoreClientAPI capi)
        where T : ClientSystem
    {
        var clientSystems = (capi.World as ClientMain).GetField<ClientSystem[]>("clientSystems");
        return clientSystems.FirstOrDefault(p => p.GetType() == typeof(T)) as T;
    }

    /// <summary>
    ///     
    /// </summary>
    /// <param name="capi">Core Client API.</param>
    /// <param name="commandName">The name of the command to unregister.</param>
    public static void UnregisterCommand(this ICoreClientAPI capi, string commandName)
    {
        var eventManager = (capi.World as ClientMain).GetField<ClientEventManager>("eventManager");
        var chatCommands = eventManager.GetField<Dictionary<string, ChatCommand>>("chatCommands");
        chatCommands.Remove(commandName);
        eventManager.SetField("chatCommands", chatCommands);
    }

    #endregion

    #region Server

    /// <summary>
    ///     Returns the <see cref="ServerMain"/> from the current <see cref="ICoreServerAPI"/> instance.
    /// </summary>
    public static ServerMain AsServerMain(this ICoreServerAPI api)
    {
        return (ServerMain)api.World;
    }

    /// <summary>
    ///     Returns the <see cref="ICoreServerAPI"/> from the current <see cref="ServerMain"/> instance.
    /// </summary>
    public static ICoreServerAPI AsApi(this ServerMain game)
    {
        return (ICoreServerAPI)game.Api;
    }

    /// <summary>
    ///     Determines whether the server is running as a dedicated server, via
    ///     VintagestoryServer.exe, or as a threaded server within Vintagestory.exe.
    /// </summary>
    [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Extension Method")]
    public static bool IsDedicatedServerProcess(this ICoreServerAPI sapi)
    {
        return Process.GetCurrentProcess().ProcessName == "VintagestoryServer";
    }

    /// <summary>
    ///     Returns a specific <see cref="ServerSystem"/> that is registered with the game engine.
    /// </summary>
    public static T GetInternalServerSystem<T>(this ICoreServerAPI sapi)
        where T : ServerSystem
    {
        var systems = ((ServerMain)sapi.World).GetField<ServerSystem[]>("Systems");
        return systems.FirstOrDefault(p => p is T) as T;
    }

    #endregion

    #region Universal

    /// <summary>
    ///     Gets the world seed.
    /// </summary>
    /// <param name="api">The core game API.</param>
    public static string GetSeed(this ICoreAPI api)
    {
        return api.World?.Seed.ToString();
    }

    /// <summary>
    ///     Converts an agnostic API to a Server-side API.
    /// </summary>
    /// <param name="api">The core game API.</param>
    public static ICoreServerAPI ForServer(this ICoreAPI api)
    {
        if (api.Side.IsClient()) return null;
        return api as ICoreServerAPI;
    }

    /// <summary>
    ///     Converts an side-agnostic API to a client-side API.
    /// </summary>
    /// <param name="api">The core game API.</param>
    public static ICoreClientAPI ForClient(this ICoreAPI api)
    {
        if (api.Side.IsServer()) return null;
        return api as ICoreClientAPI;
    }

    /// <summary>
    ///     Side-agnostic way to determine whether the current world is being played as a single-player world.
    /// </summary>
    [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Extension Method")]
    public static bool IsSinglePlayer(this ICoreAPI api)
    {
        return ApiEx.Return(
            capi => capi.IsSinglePlayer,
            sapi => !sapi.IsDedicatedServerProcess()
                    || ApiEx.ServerMain.MainSockets.Any(x => x is not DummyNetServer));
    }

    /// <summary>
    ///     Performs an action on the main thread of the application, after a given timeout.
    /// </summary>
    /// <param name="api">Core API.</param>
    /// <param name="onDelayedCallbackTick">The action to run once the delay has elapsed.</param>
    /// <param name="millisecondInterval">The delay, in millisecond, before the callback is called.</param>
    public static void RegisterDelayedCallback(this ICoreAPI api, Action<float> onDelayedCallbackTick, int millisecondInterval)
    {
        api.Event.EnqueueMainThreadTask(() =>
        {
            api.Event.RegisterCallback(onDelayedCallbackTick, millisecondInterval);
        }, "");
    }

    #endregion
}