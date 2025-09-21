using Gantry.Core.Annotation;
using Gantry.Extensions.Api;
using Vintagestory.API.Common.Entities;

namespace Gantry.Extensions;

/// <summary>
///     Extensions methods to aid working with players.
/// </summary>
public static class PlayerExtensions
{
    /// <summary>
    ///     Attempts to find an item which exposes the specified behaviour on the player.
    ///     The search order is: right hand, left hand, then any backpacks returned by
    ///     the player's inventory manager for the backpack inventory class name.
    /// </summary>
    /// <typeparam name="T">The behaviour type to look for; must derive from <see cref="CollectibleBehavior"/>.</typeparam>
    /// <param name="player">The player whose inventories will be searched.</param>
    /// <param name="slot">When this method returns, contains the <see cref="ItemSlot"/> that holds the item with the behaviour, or <c>null</c> if none was found.</param>
    /// <returns><c>true</c> if an item with the requested behaviour was found; otherwise <c>false</c>.</returns>
    public static bool TryGetItemWithBehaviour<T>(this IPlayer player, out ItemSlot? slot) where T : CollectibleBehavior
    {
        slot = null;

        if (player.Entity.RightHandItemSlot.Itemstack?.Item?.GetBehavior<T>() is not null)
        {
            slot = player.Entity.RightHandItemSlot;
        }
        if (slot is not null) return true;

        if (player.Entity.LeftHandItemSlot.Itemstack?.Item?.GetBehavior<T>() is not null)
        {
            slot = player.Entity.LeftHandItemSlot;
        }
        if (slot is not null) return true;

        foreach (var backpack in player.InventoryManager.GetOwnInventory(GlobalConstants.backpackInvClassName))
        {
            if (backpack.Itemstack?.Item?.GetBehavior<T>() is not null)
            {
                slot = backpack;
            }
            if (slot is not null) return true;
        }
        return slot is not null;
    }

    /// <summary>
    ///     Attempts to find an item of the specified usable item type on the player.
    ///     The search order is: right hand, left hand, then any backpacks returned by
    ///     the player's inventory manager for the backpack inventory class name.
    /// </summary>
    /// <typeparam name="T">The item type to look for; must derive from <see cref="Item"/>.</typeparam>
    /// <param name="player">The player whose inventories will be searched.</param>
    /// <param name="slot">When this method returns, contains the <see cref="ItemSlot"/> holding the item, or <c>null</c> if none was found.</param>
    /// <param name="item">When this method returns, contains the item instance of type <typeparamref name="T"/>, or <c>null</c> if none was found.</param>
    /// <returns><c>true</c> if an item of the requested type was found; otherwise <c>false</c>.</returns>
    public static bool TryGetItemInUsableSlot<T>(this IPlayer player, out ItemSlot? slot, out T? item) where T : Item
    {
        slot = null;
        item = null;

        if (player.Entity.RightHandItemSlot.Itemstack?.Item is T rhItem)
        {
            slot = player.Entity.RightHandItemSlot;
            item = rhItem;
        }
        if (slot is not null) return true;

        if (player.Entity.LeftHandItemSlot.Itemstack?.Item is T lhItem)
        {
            slot = player.Entity.LeftHandItemSlot;
            item = lhItem;
        }
        if (slot is not null) return true;

        foreach (var backpack in player.InventoryManager.GetOwnInventory(GlobalConstants.backpackInvClassName))
        {
            if (backpack.Itemstack?.Item is T bpItem)
            {
                slot = backpack;
                item = bpItem;
            }
            if (slot is not null) return true;
        }
        return slot is not null;
    }

    /// <summary>
    ///     Applies the specified amount of damage to the player using an internal damage source.
    /// </summary>
    /// <param name="player">The player to damage.</param>
    /// <param name="hp">The amount of hit points to apply as damage.</param>
    public static void Damage(this EntityPlayer player, float hp)
        => player.ReceiveDamage(new() { Source = EnumDamageSource.Internal }, hp);

    /// <summary>
    ///     Restores the specified amount of health to the player using a revive damage source.
    /// </summary>
    /// <param name="player">The player to heal.</param>
    /// <param name="hp">The amount of hit points to restore.</param>
    public static void Heal(this EntityPlayer player, float hp)
        => player.ReceiveDamage(new() { Source = EnumDamageSource.Revive }, hp);

    /// <summary>
    ///     Adjusts the player's stats.
    /// </summary>
    /// <param name="player">The player entity.</param>
    /// <param name="category">The cetegory to modify.</param>
    /// <param name="code">The unique identifier for the modification.</param>
    /// <param name="delta">The amount to adjust the stat by.</param>
    public static void AdjustStat(this EntityPlayer player, string category, string code, float delta)
    {
        var valuesByKey = player.Stats[category].ValuesByKey;
        if (!valuesByKey.TryGetValue(code, out var stat)) stat = new();

        player.Stats.Set(
            category: category,
            code: code,
            value: stat.Value + delta,
            persistent: true);

        var behaviour = player.GetBehavior<EntityBehaviorHealth>();
        behaviour?.MarkDirty();
    }

    /// <summary>
    ///     Determines if the entity is a player, in Spectator Mode.
    /// </summary>
    public static bool IsSpectator(this Entity entity) => entity is EntityPlayer
    {
        Player.WorldData.CurrentGameMode: EnumGameMode.Spectator
    };

    /// <summary>
    ///     Sends a chat message to the player, using the current chat channel.
    /// </summary>
    public static void SendMessage(this IServerPlayer player, string message)
    {
        player.SendMessage(GlobalConstants.CurrentChatGroup, message, EnumChatType.Notification);
    }

    /// <summary>
    ///     Sends a chat message to the player, using the current chat channel.
    /// </summary>
    public static void ShowChatMessage(this IPlayer player, string message)
    {
        player.Entity.Api.Side.Invoke(
            () => ((IClientPlayer)player).ShowChatNotification(message), 
            () => ((IServerPlayer)player).SendMessage(message));
    }

    /// <summary>
    ///     Thread-Safe.
    ///     Shows a client side only chat message in the current chat channel. Does not execute client commands.
    /// </summary>
    /// <param name="api">The core game API this method was called from.</param>
    /// <param name="message">The message to show to the player.</param>
    public static void EnqueueShowChatMessage(this ICoreClientAPI api, string message)
    {
        (api.World as ClientMain)?.EnqueueShowChatMessage(message);
    }

    /// <summary>
    ///     Thread-Safe.
    ///     Shows a client side only chat message in the current chat channel. Does not execute client commands.
    /// </summary>
    /// <param name="game">The core game API this method was called from.</param>
    /// <param name="message">The message to show to the player.</param>
    public static void EnqueueShowChatMessage(this ClientMain game, string message)
    {
        game?.EnqueueMainThreadTask(() => game.ShowChatMessage(message), "");
    }

    /// <summary>
    ///     Sets the movement speed for the player. Default is 1.
    /// </summary>
    /// <param name="player"></param>
    /// <param name="speed"></param>
    [ServerSide]
    public static void SetMovementSpeed(this IPlayer player, float speed = 1f)
    {
        player.WorldData.MoveSpeedMultiplier = speed;
        player.WorldData.EntityControls.MovespeedMultiplier = speed;
    }

    /// <summary>
    ///     Sets the movement speed for the player. Default is 1.
    /// </summary>
    /// <param name="player"></param>
    /// <param name="axisLock"></param>
    [ServerSide]
    public static void SetMovementPlaneLock(this IPlayer player, EnumFreeMovAxisLock axisLock = EnumFreeMovAxisLock.None)
    {
        player.WorldData.FreeMovePlaneLock = axisLock;
        player.WorldData.EntityControls.FlyPlaneLock = axisLock;
    }

    /// <summary>
    ///     Deactivate Creative Flight.
    /// </summary>
    /// <param name="player"></param>
    [ServerSide]
    public static void DeactivateFlightMode(this IServerPlayer player)
    {
        var sapi = player.Entity.Api.ForServer()!;
        var playerPos = player.Entity.Pos.XYZ.AsBlockPos;
        var y = sapi.WorldManager.GetSurfacePosY(playerPos.X, playerPos.Z);
        player.Entity.PositionBeforeFalling = playerPos.With(p => p.Y = y ?? p.Y).ToVec3d();

        player.WorldData.FreeMove = false;
        player.Entity.Properties.FallDamage = true;
        player.SetMovementPlaneLock(EnumFreeMovAxisLock.None);
        player.SetMovementSpeed(1f);
        player.BroadcastPlayerData(false);
    }

    /// <summary>
    ///     Activate Creative Flight.
    /// </summary>
    [ServerSide]
    public static void ActivateFlightMode(this IPlayer player, 
        float moveSpeed = 1, EnumFreeMovAxisLock axisLock = EnumFreeMovAxisLock.None)
    {
        player.WorldData.FreeMove = true;
        player.Entity.Properties.FallDamage = false;
        player.SetMovementPlaneLock(axisLock);
        player.SetMovementSpeed(moveSpeed);
        ((IServerPlayer)player).BroadcastPlayerData(false);
    }
}