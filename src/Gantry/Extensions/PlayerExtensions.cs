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