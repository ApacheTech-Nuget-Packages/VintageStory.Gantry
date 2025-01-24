namespace Gantry.Core.Extensions;

/// <summary>
///     Extension methods to help register classes with the game engine.
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class ClassRegistryExtensions
{
    /// <summary>
    ///     Registers a new Item class. <br/>
    ///     Must happen before any blocks are loaded. <br/>
    ///     Must be registered on both the client, and server.
    /// </summary>
    /// <typeparam name="T">The type of the Item to register.</typeparam>
    /// <param name="api">The game's internal API.</param>
    public static void RegisterItem<T>(this ICoreAPICommon api)
    {
        var type = typeof(T);
        api.RegisterItemClass(type.Name, type);
    }

    /// <summary>
    ///     Registers a new Item class. <br/>
    ///     Must happen before any blocks are loaded. <br/>
    ///     Must be registered on both the client, and server.
    /// </summary>
    /// <typeparam name="T">The type of the Item to register.</typeparam>
    /// <param name="api">The game's internal API.</param>
    /// <param name="name">The name to give to the item.</param>
    public static void RegisterItem<T>(this ICoreAPI api, string name = null)
    {
        name ??= nameof(T);
        api.RegisterItemClass(name, typeof(T));
    }

    /// <summary>
    ///     Registers a new Block class. <br/>
    ///     Must happen before any blocks are loaded. <br/>
    ///     Must be registered on both the client, and server.
    /// </summary>
    /// <typeparam name="T">The type of the Block to register.</typeparam>
    /// <param name="api">The game's internal API.</param>
    public static void RegisterBlock<T>(this ICoreAPICommon api)
    {
        var type = typeof(T);
        api.RegisterBlockClass(type.Name, type);
    }

    /// <summary>
    ///     Registers a new Block class. <br/>
    ///     Must happen before any blocks are loaded. <br/>
    ///     Must be registered on both the client, and server.
    /// </summary>
    /// <typeparam name="T">The type of the Block to register.</typeparam>
    /// <param name="api">The game's internal API.</param>
    public static void RegisterBlockEntity<T>(this ICoreAPICommon api)
    {
        var type = typeof(T);
        api.RegisterBlockEntityClass(type.Name, type);
    }

    /// <summary>
    ///     Registers a new Block Behaviour. <br/>
    ///     Must happen before any blocks are loaded. <br/>
    ///     Must be registered on both the client, and server.
    /// </summary>
    /// <typeparam name="T">The type of the Block Behaviour to register.</typeparam>
    /// <param name="api">The game's internal API.</param>
    /// <param name="friendlyName">A friendly name to give to the behaviour.</param>
    public static void RegisterBlockBehaviour<T>(this ICoreAPICommon api, string friendlyName = null)
    {
        var type = typeof(T);
        api.RegisterBlockBehaviorClass(friendlyName?.IfNullOrEmpty(type.Name), type);
    }

    /// <summary>
    ///     Registers a new BlockEntity Behaviour. <br/>
    ///     Must happen before any blocks are loaded. <br/>
    ///     Must be registered on both the client, and server.
    /// </summary>
    /// <typeparam name="T">The type of the BlockEntity Behaviour to register.</typeparam>
    /// <param name="api">The game's internal API.</param>
    public static void RegisterBlockEntityBehaviour<T>(this ICoreAPICommon api)
    {
        var type = typeof(T);
        api.RegisterBlockEntityBehaviorClass(type.Name, type);
    }

    /// <summary>
    ///     Registers a new Block Behaviour. <br/>
    ///     Must happen before any blocks are loaded. <br/>
    ///     Must be registered on both the client, and server.
    /// </summary>
    /// <typeparam name="T">The type of the Block Behaviour to register.</typeparam>
    /// <param name="api">The game's internal API.</param>
    public static void RegisterEntity<T>(this ICoreAPICommon api)
    {
        var type = typeof(T);
        api.RegisterEntity(type.Name, type);
    }

    /// <summary>
    ///     Registers a new Entity Behaviour. <br/>
    ///     Must be registered on both the client, and server.
    /// </summary>
    /// <typeparam name="T">The type of the Entity Behaviour to register.</typeparam>
    /// <param name="api">The game's internal API.</param>
    public static void RegisterEntityBehaviour<T>(this ICoreAPICommon api)
    {
        var type = typeof(T);
        api.RegisterEntityBehaviorClass(type.Name, type);
    }

    /// <summary>
    ///     Registers a new Crop Behaviour. <br/>
    ///     Must be registered on both the client, and server.
    /// </summary>
    /// <typeparam name="T">The type of the Crop Behaviour to register.</typeparam>
    /// <param name="api">The game's internal API.</param>
    public static void RegisterCropBehaviour<T>(this ICoreAPICommon api)
    {
        var type = typeof(T);
        api.RegisterCropBehavior(type.Name, type);
    }

    /// <summary>
    ///     Registers a new Collectible Behaviour. <br/>
    ///     Must be registered on both the client, and server.
    /// </summary>
    /// <typeparam name="T">The type of the Collectible Behaviour to register.</typeparam>
    /// <param name="api">The game's internal API.</param>
    public static void RegisterCollectibleBehaviour<T>(this ICoreAPICommon api)
    {
        var type = typeof(T);
        api.RegisterCollectibleBehaviorClass(type.Name, type);
    }


    /// <summary>
    ///     Registers a new Collectible Behaviour. <br/>
    ///     Must be registered on both the client, and server.
    /// </summary>
    /// <typeparam name="T">The type of the Collectible Behaviour to register.</typeparam>
    /// <param name="api">The game's internal API.</param>
    /// <param name="name">The name to give to the behaviour.</param>
    public static void RegisterCollectibleBehaviour<T>(this ICoreAPI api, string name = null)
    {
        name ??= nameof(T);
        api.RegisterCollectibleBehaviorClass(name, typeof(T));
    }

    /// <summary>
    ///     Adds a specific block behaviour to all blocks of a given type in the block list.
    /// </summary>
    /// <typeparam name="TBlock">The type of the block to which the behaviour should be added.</typeparam>
    /// <typeparam name="TBehaviour">The type of the behaviour to add.</typeparam>
    /// <param name="blocks">The list of blocks to process.</param>
    /// <param name="behaviourFactory">A factory function to create the behaviour for each block.</param>
    public static void AddBehaviourToBlocks<TBlock, TBehaviour>(
        this IList<Block> blocks,
        System.Func<Block, TBehaviour> behaviourFactory)
        where TBlock : Block
        where TBehaviour : BlockBehavior
    {
        foreach (var block in blocks.OfType<TBlock>())
        {
            var behaviour = behaviourFactory(block);
            block.BlockBehaviors = block.BlockBehaviors.Append(behaviour).ToArray();
        }
    }
}