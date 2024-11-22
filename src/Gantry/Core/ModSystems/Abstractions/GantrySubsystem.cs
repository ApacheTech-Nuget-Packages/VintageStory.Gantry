using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace Gantry.Core.ModSystems.Abstractions;

/// <summary>
///     Represents a base subsystem for the Gantry, providing methods for lifecycle events
///     such as loading, starting, and asset finalisation on both client and server sides.
/// </summary>
public abstract class GantrySubsystem
{
    /// <summary>
    ///     Gets the mod this mod system is part of.
    /// </summary>
    public static Mod Mod => ModEx.Mod;

    /// <summary>
    ///     Determines whether this mod should be loaded for the specified application side.
    /// </summary>
    /// <param name="forSide">The side (client or server) to check.</param>
    /// <returns>
    ///     <c>true</c> if the mod should load; otherwise, <c>false</c>.
    /// </returns>
    public virtual bool ShouldLoad(EnumAppSide forSide)
    {
        return true;
    }

    /// <summary>
    ///     Provides the execution order for this mod. Lower values are executed earlier.
    /// </summary>
    /// <returns>
    ///     A double value representing the execution order. Default is <c>0.1</c>.
    /// </returns>
    public virtual double ExecuteOrder()
    {
        return 0.1;
    }

    /// <summary>
    ///     Called during the initial mod loading phase, before any mod receives the <see cref="Start" /> call.
    /// </summary>
    /// <param name="api">The API interface for the core game.</param>
    public virtual void StartPre(ICoreAPI api)
    {
    }

    /// <summary>
    ///     Called on both client and server after <see cref="StartPre" /> but before assets are loaded.
    ///     Typically used to register events, network packets, or initialise core mod components.
    /// </summary>
    /// <param name="api">The API interface for the core game.</param>
    public virtual void Start(ICoreAPI api)
    {
    }

    /// <summary>
    ///     Called after assets have been read from disk and patched. Use this to load assets or perform
    ///     early setup operations. Blocks and items may not yet be fully registered, depending on the
    ///     execute order.
    /// </summary>
    /// <param name="api">The API interface for the core game.</param>
    public virtual void AssetsLoaded(ICoreAPI api)
    {
    }

    /// <summary>
    ///     Finalises asset-related operations after all blocks, items, and other mod elements
    ///     have been registered.
    /// </summary>
    /// <param name="api">The API interface for the core game.</param>
    public virtual void AssetsFinalize(ICoreAPI api)
    {
    }

    /// <summary>
    ///     Full start method for the client side. At this stage, assets from the server
    ///     (e.g., blocks, items, and recipes) may not yet be registered in multiplayer games.
    /// </summary>
    /// <param name="api">The client-specific API interface.</param>
    public virtual void StartClientSide(ICoreClientAPI api)
    {
    }

    /// <summary>
    ///     Full start method for the server side. Use this to initialise server-specific
    ///     logic and behaviours.
    /// </summary>
    /// <param name="api">The server-specific API interface.</param>
    public virtual void StartServerSide(ICoreServerAPI api)
    {
    }

    /// <summary>
    ///     Disposes of the mod. If runtime reloading is supported, implement this method
    ///     to unregister listeners or handlers.
    /// </summary>
    public virtual void Dispose()
    {
    }
}