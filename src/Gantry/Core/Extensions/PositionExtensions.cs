﻿#nullable enable
using Gantry.Core.Extensions.Helpers;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;

namespace Gantry.Core.Extensions;

/// <summary>
///     Extension methods to aid working with gameworld positions.
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class PositionExtensions
{
    /// <summary>
    ///     Gets the position relative to spawn, given an absolute position within the game world.
    /// </summary>
    /// <param name="pos">The absolute position of the block being queried.</param>
    public static BlockPos RelativeToSpawn(this BlockPos pos)
    {
        return pos.SubCopy(ApiEx.Current.World.DefaultSpawnPosition.XYZ.AsBlockPos).With(p => p.Y = pos.Y);
    }

    /// <summary>
    ///     Gets the absolute position, given an relative position within the game world.
    /// </summary>
    /// <param name="pos">The relative position of the block being queried.</param>
    /// <param name="spawnPosition">The spawn position to use as the relative origin.</param>
    public static BlockPos ToAbsolute(this BlockPos pos, BlockPos? spawnPosition = null)
    {
        spawnPosition ??= ApiEx.Current.World.DefaultSpawnPosition.XYZ.AsBlockPos;
        return spawnPosition.AddCopy(pos.AsVec3i).With(p => p.Y = pos.Y);
    }

    /// <summary>
    ///     Gets the position relative to spawn, given an absolute position within the game world.
    /// </summary>
    /// <param name="pos">The absolute position of the block being queried.</param>
    public static Vec3d RelativeToSpawn(this Vec3d pos)
    {
        return pos.SubCopy(ApiEx.Current.World.DefaultSpawnPosition.XYZ).With(p => p.Y = pos.Y);
    }

    /// <summary>
    ///     Gets the absolute position, given an relative position within the game world.
    /// </summary>
    /// <param name="pos">The relative position of the block being queried.</param>
    /// <param name="spawnPosition">The spawn position to use as the relative origin.</param>
    public static Vec3d ToAbsolute(this Vec3d pos, Vec3d? spawnPosition = null)
    {
        spawnPosition ??= ApiEx.Current.World.DefaultSpawnPosition.XYZ;
        return spawnPosition.AddCopy(pos).With(p => p.Y = pos.Y);
    }

    /// <summary>
    ///     Ensures that a <see cref="BlockPos"/> is inside the world borders.
    /// </summary>
    public static BlockPos ClampToWorldBounds(this BlockPos blockPos, IBlockAccessor blockAccessor)
    {
        blockPos.X = GameMath.Clamp(blockPos.X, 0, blockAccessor.MapSizeX);
        blockPos.Y = GameMath.Clamp(blockPos.Y, 0, blockAccessor.MapSizeY);
        blockPos.Z = GameMath.Clamp(blockPos.Z, 0, blockAccessor.MapSizeZ);
        return blockPos;
    }

    /// <summary>
    ///     Generates a random position within a specified range of an origin position.
    /// </summary>
    /// <param name="origin">The origin position.</param>
    /// <param name="horizontalRadius">The radius away from the origin to use as the upper and lower bounds for the X and Z coordinates of the returned position.</param>
    /// <param name="verticalRadius">The radius away from the origin to use as the upper and lower bounds for the Y coordinates of the returned position.</param>
    /// <returns>A <see cref="Vec3d"/> representing a position in the game world, a random distance away from the origin position.</returns>
    public static Vec3d GetRandomPositionInRange(this Vec3d origin, int horizontalRadius, int verticalRadius = 0)
    {
        var x = RandomEx.RandomValueBetween(-horizontalRadius, horizontalRadius);
        var y = RandomEx.RandomValueBetween(-verticalRadius, verticalRadius);
        var z = RandomEx.RandomValueBetween(-horizontalRadius, horizontalRadius);
        return origin.AddCopy(x, y, z);
    }

    /// <summary>
    ///     Gets the surface level at the specified block position. This is the highest point on the Y axis, where the block is solid, from above.
    /// </summary>
    /// <param name="pos">The position.</param>
    /// <param name="blockAccessor">The block accessor for the current world.</param>
    /// <returns>A <see cref="int"/> value, representing the highest Y value with a solid block underneath it.</returns>
    public static int GetSurfaceLevel(this BlockPos pos, IBlockAccessor blockAccessor)
    {
        var maxY = blockAccessor.GetTerrainMapheightAt(pos);
        var minPos = new BlockPos(pos.X, 1, pos.Z, Dimensions.NormalWorld);
        var maxPos = new BlockPos(pos.X, blockAccessor.MapSizeY, pos.Z, Dimensions.NormalWorld);
        blockAccessor.WalkBlocks(minPos, maxPos, (block, _, y, _) =>
        {
            if (!block.SideSolid[BlockFacing.indexUP]) return;
            if (block.LiquidLevel == 7) return;
            maxY = y;
        });
        return maxY;
    }

    /// <summary>
    ///     Checks to see whether the entity will collide with anything at a given position in the world.
    /// </summary>
    /// <param name="entity">The entity in question.</param>
    /// <param name="position">The position for which to check for collisions.</param>
    /// <returns>Returns <c>true</c>, if the entity will collide with something at the given position; otherwise, <c>false</c>.</returns>
    public static bool CollisionCheck(this Entity entity, Vec3d position)
    {
        return entity.World
            .CollisionTester
            .GetCollidingCollisionBox(entity.World.BlockAccessor, entity.CollisionBox, position, false) == null;
    }

    /// <summary>
    ///    Calculates the Euclidean distance between the current position and the specified position.
    /// </summary>
    /// <param name="aPos">The current position to calculate the distance from.</param>
    /// <param name="bPos">The position to calculate the distance to.</param>
    /// <returns>The Euclidean distance between the two positions.</returns>
    public static double DistanceTo(this EntityPos aPos, Vec3d bPos)
        => Math.Sqrt(aPos.SquareDistanceTo(bPos));

    /// <summary>
    ///    Calculates the Euclidean distance between the current position and the specified position,
    ///    rounded to the specified precision.
    /// </summary>
    /// <param name="aPos">The current position to calculate the distance from.</param>
    /// <param name="bPos">The position to calculate the distance to.</param>
    /// <param name="precision">The number of decimal places to round the result to.</param>
    /// <returns>The Euclidean distance between the two positions, rounded to the specified precision.</returns>
    public static double RoundedDistanceTo(this EntityPos aPos, Vec3d bPos, int precision)
        => Math.Round(aPos.DistanceTo(bPos), precision);
}