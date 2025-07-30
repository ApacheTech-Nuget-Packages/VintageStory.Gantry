using Gantry.Extensions;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;

namespace Gantry.GameContent.World;

/// <summary>
///     A self validating block position, clamped to the bounds of the world.
/// </summary>
/// <seealso cref="BlockPos" />
public class ClampedBlockPos : BlockPos
{
    private readonly IWorldAccessor _world;

#pragma warning disable CS0618 // Type or member is obsolete
    private ClampedBlockPos(BlockPos pos, IWorldAccessor world)
    {
        (X, Y, Z, dimension) = (pos.X, pos.Y, pos.Z, pos.dimension);
        _world = world;
    }
#pragma warning restore CS0618 // Type or member is obsolete

    /// <summary>
    ///     Initialises a new instance of the <see cref="ClampedBlockPos"/> class.
    /// </summary>
    /// <param name="pos">The position to decorate.</param>
    /// <param name="world">The accessor to use to determine world boundaries.</param>
    public static ClampedBlockPos FromBlockPos(BlockPos pos, IWorldAccessor world)
        => new(pos, world);

    /// <summary>
    ///     Initialises a new instance of the <see cref="ClampedBlockPos"/> class.
    /// </summary>
    /// <param name="pos">The position to decorate.</param>
    /// <param name="world">The accessor to use to determine world boundaries.</param>
    public static ClampedBlockPos FromBlockPos(Vec3i pos, IWorldAccessor world)
        => new(pos.AsBlockPos, world);

    /// <summary>
    ///     Initialises a new instance of the <see cref="ClampedBlockPos"/> class.
    /// </summary>
    /// <param name="pos">The position to decorate.</param>
    /// <param name="world">The accessor to use to determine world boundaries.</param>
    public static ClampedBlockPos FromBlockPos(EntityPos pos, IWorldAccessor world)
        => new(pos.AsBlockPos, world);

    /// <summary>
    ///     Initialises a new instance of the <see cref="ClampedBlockPos"/> class.
    /// </summary>
    /// <param name="world">The accessor to use to determine world boundaries.</param>
    public static ClampedBlockPos WorldSpawn(IWorldAccessor world)
        => new(world.DefaultSpawnPosition.AsBlockPos, world);

    /// <summary>
    ///     Initialises a new instance of the <see cref="ClampedBlockPos"/> class.
    /// </summary>
    /// <param name="player">The player to use to determine the position.</param>
    public static ClampedBlockPos FromPlayerPos(IPlayer player)
        => new(player.Entity.Pos.AsBlockPos.Copy(), player.Entity.Api.World);

    /// <summary>
    ///     Decreases the value on the X axis, by 1. Clamps the resulting position within the world bounds.
    /// </summary>
    public void DecrementX() => SetX(X - 1);

    /// <summary>
    ///     Decreases the value on the Y axis, by 1. Clamps the resulting position within the world bounds.
    /// </summary>
    public void DecrementY() => SetY(Y - 1);

    /// <summary>
    ///     Decreases the value on the Z axis, by 1. Clamps the resulting position within the world bounds.
    /// </summary>
    public void DecrementZ() => SetZ(Z - 1);

    /// <summary>
    ///     Increases the value on the X axis, by 1. Clamps the resulting position within the world bounds.
    /// </summary>
    public void IncrementX() => SetX(X + 1);

    /// <summary>
    ///     Increases the value on the Y axis, by 1. Clamps the resulting position within the world bounds.
    /// </summary>
    public void IncrementY() => SetY(Y + 1);
    /// <summary>
    ///     Increases the value on the Z axis, by 1. Clamps the resulting position within the world bounds.
    /// </summary>
    public void IncrementZ() => SetZ(Z + 1);

    /// <summary>
    ///     Sets the value on the X axis. Clamps the resulting position within the world bounds.
    /// </summary>
    public void SetX(int value) => Clamp(() => X = value);

    /// <summary>
    ///     Sets the value on the Y axis. Clamps the resulting position within the world bounds.
    /// </summary>
    public void SetY(int value) => Clamp(() => Y = value);

    /// <summary>
    ///     Sets the value on the Z axis. Clamps the resulting position within the world bounds.
    /// </summary>
    public void SetZ(int value) => Clamp(() => Z = value);

    /// <summary>
    ///     Sets the specified position.
    /// </summary>
    public new void Set(BlockPos pos) => Clamp(() => base.Set(pos));

    private void Clamp(Action operation)
    {
        operation();
        this.ClampToWorldBounds(_world.BlockAccessor);
    }
}