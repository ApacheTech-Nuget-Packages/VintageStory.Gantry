namespace Gantry.Core.Abstractions.ModSystems;

/// <summary>
///     Base interface for a system that is part of a code mod. Handles setup, registration, and lifecycle events for mod systems.
///     Mods can be split into multiple systems or have just one.
/// </summary>
public interface IHostedModSystem : IDisposable
{
    /// <summary>
    ///     The <see cref="Mod"/> this mod system is part of.
    /// </summary>
    Mod Mod { get; }

    /// <summary>
    ///     The Gantry Core API for the current mod and app side.
    /// </summary>
    ICoreGantryAPI Core { get; }

    /// <summary>
    ///     Sets the Gantry Core API for the current mod and app side.
    /// </summary>
    internal void SetCore(ICoreGantryAPI core);

    /// <summary>
    ///     Returns whether this mod should be loaded for the given <see cref="ICoreAPI"/> instance.
    /// </summary>
    /// <param name="api">The core API instance.</param>
    /// <returns><c>true</c> if the mod should be loaded; otherwise, <c>false</c>.</returns>
    public virtual bool ShouldLoad(ICoreAPI api) => ShouldLoad(api.Side);

    /// <summary>
    ///     Returns whether this mod should be loaded for the given app side, called by <see cref="ShouldLoad(ICoreAPI)"/>.
    /// </summary>
    /// <param name="forSide">The application side (client or server).</param>
    /// <returns><c>true</c> if the mod should be loaded; otherwise, <c>false</c>.</returns>
    public virtual bool ShouldLoad(EnumAppSide forSide) => true;

    /// <summary>
    ///     If you need mods to be executed in a certain order, adjust this method's return value.
    ///     The server will call each mod's <see cref="StartPre(ICoreAPI)"/> and <see cref="Start(ICoreAPI)"/> methods in ascending order
    ///     of each mod's execute order value. Thus, as long as every mod registers its event handlers in the <see cref="Start(ICoreAPI)"/> method,
    ///     all event handlers will be called in the same execution order.
    ///
    ///     <para>Default execute order of some survival mod parts:</para>
    ///     <list type="bullet">
    ///         <item>Worldgen:</item>
    ///         <item>GenTerra: 0</item>
    ///         <item>RockStrata: 0.1</item>
    ///         <item>Deposits: 0.2</item>
    ///         <item>Caves: 0.3</item>
    ///         <item>Blocklayers: 0.4</item>
    ///         <item>AssetsLoaded event:</item>
    ///         <item>JsonPatch loader: 0.05</item>
    ///         <item>Load hardcoded mantle block: 0.1</item>
    ///         <item>Block and Item Loader: 0.2</item>
    ///         <item>Recipes (Smithing, Knapping, Clayforming, Grid recipes, Alloys) Loader: 1</item>
    ///     </list>
    ///     <para>Default is <c>0.1</c>.</para>
    /// </summary>
    /// <returns>The execution order value.</returns>
    public virtual double ExecuteOrder() => 0.1;

    /// <summary>
    ///     Called during initial mod loading, before any mod receives the call to <see cref="Start(ICoreAPI)"/>.
    /// </summary>
    /// <param name="api">The core API instance.</param>
    public virtual void StartPre(ICoreAPI api)
    {
    }

    /// <summary>
    ///     Start method, called on both server and client after all mods have already received a call to <see cref="StartPre(ICoreAPI)"/>, 
    ///     but before Blocks/Items/Entities/Recipes etc. are loaded and some time before <c>StartServerSide</c> / <c>StartClientSide</c>.
    ///     Typically used to register for events and network packets etc.
    ///     Typically also used in a mod's core to register the classes for your blocks, items, entities, blockentities, behaviours etc., prior to loading assets.
    ///     <para>Do not make calls to <c>api.Assets</c> at this stage, the assets may not be found, resulting in errors (even if the json file exists on disk). Use <see cref="AssetsLoaded(ICoreAPI)"/> stage instead.</para>
    /// </summary>
    /// <param name="api">The core API instance.</param>
    public virtual void Start(ICoreAPI api)
    {
    }

    /// <summary>
    ///     Called on the server or the client; implementing code may need to check which side it is.
    ///     On a server, called only after all mods have received <see cref="Start(ICoreAPI)"/>, and after asset JSONs have been read from disk and patched, but before runphase <c>ModsAndConfigReady</c>.
    ///     <para>Asset files are now available to load using <c>api.Assets.TryGet()</c> calls or similar.</para>
    ///     <para>If your execute order is below 0.2, blocks and items are not yet registered at this point; if below 0.6, recipes are not yet registered.</para>
    /// </summary>
    /// <param name="api">The core API instance.</param>
    public virtual void AssetsLoaded(ICoreAPI api)
    {
    }

    /// <summary>
    ///     When called on a server, all <c>Block.OnLoaded()</c> methods etc. have already been called; this is for any final asset set-up steps to be done after that.
    ///     See VSSurvivalMod system BlockReinforcement.cs for an example.
    /// </summary>
    /// <param name="api">The core API instance.</param>
    public virtual void AssetsFinalize(ICoreAPI api)
    {
    }

    /// <summary>
    ///     Full start to the mod on the client side.
    ///     Note, in multiplayer games, the server assets (blocks, items, entities, recipes) have not yet been received and so no blocks etc. are yet registered.
    ///     For code that must run only after we have blocks, items, entities and recipes all registered and loaded, add your method to the event <c>BlockTexturesLoaded</c>.
    /// </summary>
    /// <param name="api">The client API instance.</param>
    public virtual void StartClientSide(ICoreClientAPI api)
    {
    }

    /// <summary>
    ///     Full start to the mod on the server side.
    ///     Note: preferably, your code which adds or updates behaviours or attributes or other fixed properties of any block, item or entity, should have been run before now.
    ///     For example, code which needs to do that could be placed in an overridden <see cref="AssetsFinalize(ICoreAPI)"/> method. See VSSurvivalMod system BlockReinforcement.cs for an example.
    /// </summary>
    /// <param name="api">The server API instance.</param>
    public virtual void StartServerSide(ICoreServerAPI api)
    {
    }
}