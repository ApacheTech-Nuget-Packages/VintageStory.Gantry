using Gantry.Core.Extensions;
using JetBrains.Annotations;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;

namespace Gantry.Core.GameContent.World;

/// <summary>
///     A self validating block position, clamped to the bounds of the world.
/// </summary>
/// <seealso cref="BlockPos" />
[UsedImplicitly]
public class ClampedBlockPos : BlockPos
{
#pragma warning disable CS0618 // Type or member is obsolete
    private ClampedBlockPos(BlockPos pos) => (X, Y, Z, dimension) = (pos.X, pos.Y, pos.Z, pos.dimension);
#pragma warning restore CS0618 // Type or member is obsolete

    /// <summary>
    ///     Initialises a new instance of the <see cref="ClampedBlockPos"/> class.
    /// </summary>
    /// <param name="pos">The position to decorate.</param>
    public static ClampedBlockPos FromBlockPos(BlockPos pos)
        => new(pos);

    /// <summary>
    ///     Initialises a new instance of the <see cref="ClampedBlockPos"/> class.
    /// </summary>
    /// <param name="pos">The position to decorate.</param>
    public static ClampedBlockPos FromBlockPos(Vec3i pos)
        => new(pos.AsBlockPos);

    /// <summary>
    ///     Initialises a new instance of the <see cref="ClampedBlockPos"/> class.
    /// </summary>
    /// <param name="pos">The position to decorate.</param>
    public static ClampedBlockPos FromBlockPos(EntityPos pos)
        => new(pos.AsBlockPos);

    /// <summary>
    ///     Initialises a new instance of the <see cref="ClampedBlockPos"/> class.
    /// </summary>
    public static ClampedBlockPos WorldSpawn()
        => new(ApiEx.Current.World.DefaultSpawnPosition.AsBlockPos);

    /// <summary>
    ///     Initialises a new instance of the <see cref="ClampedBlockPos"/> class.
    /// </summary>
    public static ClampedBlockPos FromPlayerPos()
        => new(ApiEx.Client.World.Player.Entity.Pos.AsBlockPos.Copy());

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
        this.ClampToWorldBounds(ApiEx.Current.World.BlockAccessor);
    }
}