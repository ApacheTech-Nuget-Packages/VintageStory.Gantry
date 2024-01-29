using Gantry.Core.Abstractions;
using JetBrains.Annotations;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;

namespace Gantry.Core.Extensions.Api;

/// <summary>
///     Extension methods that aid scanning for blocks, and block entities.
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class BlockPosExtensions
{
    /// <summary>
    ///     Gets the block at the specified position within the gameworld.
    /// </summary>
    /// <typeparam name="TBlock">The type of the block.</typeparam>
    /// <param name="this">The <see cref="BlockPos"/> to find the block at.</param>
    /// <param name="layer">Each block pos can house a liquid, and a solid. Defaults to solid blocks.</param>
    public static TBlock GetBlock<TBlock>(this BlockPos @this, BlockLayer layer = BlockLayer.Default)
        where TBlock : Block
    {
        return ApiEx.Current.World.BlockAccessor.GetBlock(@this, (int)layer) as TBlock;
    }

    /// <summary>
    ///     Gets the block entity at the specified position within the gameworld.
    /// </summary>
    /// <typeparam name="TBlockEntity">The type of the block.</typeparam>
    /// <param name="this">The <see cref="BlockPos"/> to find the block at.</param>
    public static TBlockEntity GetBlockEntity<TBlockEntity>(this BlockPos @this)
        where TBlockEntity : BlockEntity
    {
        return ApiEx.Current.World.BlockAccessor.GetBlockEntity(@this) as TBlockEntity;
    }
}



/// <summary>
///     Extension methods that aid scanning for blocks, and block entities.
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class WorldAccessorExtensions
{
    /// <summary>
    ///     A method to iterate over blocks in an area. Less overhead than when calling GetBlock(pos) many times. Currently used for more efficient collision testing.
    /// </summary>
    /// <param name="walker"></param>
    /// <param name="minPos"></param>
    /// <param name="maxPos"></param>
    /// <param name="onBlockMap"></param>
    /// <param name="onBlockPredicate">The method in which you want to check for the block, whatever it may be.</param>
    /// <param name="centreOrder">If true, the blocks will be ordered by the distance to the center position</param>
    public static TResult WalkBlocks<TResult>(
        this IBlockAccessor walker,
        BlockPos minPos,
        BlockPos maxPos,
        System.Func<Block, BlockPos, bool> onBlockPredicate,
        System.Func<Block, BlockPos, TResult> onBlockMap,
        bool centreOrder)
        where TResult : class
    {
        var result = default(TResult);
        var found = false;

        walker.WalkBlocks(minPos, maxPos, (b, x, y, z) =>
        {
            if (found) return;
            var pos = new BlockPos(x, y, z, Dimensions.NormalWorld);
            if (!onBlockPredicate(b, pos)) return;
            result = onBlockMap(b, pos);
            found = true;
        }, centreOrder);

        return result;
    }

    /// <summary>
    ///     Gets the nearest block, of a specified type, given an origin <see cref="BlockPos"/>.
    /// </summary>
    /// <typeparam name="TBlock">The type of the block entity.</typeparam>
    /// <param name="world">The world to scan for blocks in.</param>
    /// <param name="origin">The origin position.</param>
    /// <param name="horizontalRange">The horizontal (X/Z) range to scan.</param>
    /// <param name="verticalRange">The vertical (Y) range to scan.</param>
    /// <param name="predicate">A custom filter to narrow the focus of the search.</param>
    public static BlockPos GetNearestBlock<TBlock>(
        this IWorldAccessor world,
        BlockPos origin,
        float horizontalRange,
        float verticalRange,
        System.Func<TBlock, bool> predicate) where TBlock : Block
    {
        var walker = world.GetBlockAccessorPrefetch(false, false);
        var (minPos, maxPos) = origin.GetBlockRange(horizontalRange, verticalRange);
        walker.PrefetchBlocks(minPos, maxPos);

        var blockPos = walker.WalkBlocks(minPos, maxPos,
            (b, _) => b is TBlock block && predicate(block),
            (_, pos) => pos,
            true);

        return blockPos;
    }

    private static (BlockPos MinPos, BlockPos MaxPos) GetBlockRange(
        this BlockPos origin, float horizontalRange, float verticalRange)
    {
        var minPos = origin
            .AddCopy(-horizontalRange, -verticalRange, -horizontalRange)
            .ClampToWorldBounds();

        var maxPos = origin
            .AddCopy(horizontalRange, verticalRange, horizontalRange)
            .ClampToWorldBounds();

        return (minPos, maxPos);
    }

    /// <summary>
    ///     Gets the nearest block entity, of a specified type, given an origin <see cref="BlockPos"/>.
    /// </summary>
    /// <typeparam name="TBlockEntity">The type of the block entity.</typeparam>
    /// <param name="world">The world to scan for blocks in.</param>
    /// <param name="origin">The origin position.</param>
    /// <param name="horRange">The horizontal (X/Z) range to scan.</param>
    /// <param name="vertRange">The vertical (Y) range to scan.</param>
    /// <param name="predicate">A custom filter to narrow the focus of the search.</param>
    public static TBlockEntity GetNearestBlockEntity<TBlockEntity>(this IWorldAccessor world, BlockPos origin,
        float horRange, float vertRange, System.Func<TBlockEntity, bool> predicate) where TBlockEntity : BlockEntity
    {
        var walker = world.GetBlockAccessorPrefetch(false, false);
        TBlockEntity blockEntity = null;
        var minPos = origin.AddCopy(-horRange, -vertRange, -horRange).ClampToWorldBounds();
        var maxPos = origin.AddCopy(horRange, vertRange, horRange).ClampToWorldBounds();
        walker.PrefetchBlocks(minPos, maxPos);

        world.BlockAccessor.WalkBlocks(minPos, maxPos, (_, x, y, z) =>
        {
            var position = new BlockPos(x, y, z, Dimensions.NormalWorld);
            var entity = walker.GetBlockEntity(position);
            if (entity is null) return;
            if (!(entity.GetType() == typeof(TBlockEntity))) return;
            if (predicate((TBlockEntity)entity)) blockEntity = (TBlockEntity)entity;
        }, true);
        return blockEntity;
    }

    /// <summary>
    ///     Gets the nearest block entity, of a specified type, given an origin <see cref="BlockPos"/>.
    /// </summary>
    /// <typeparam name="TBlockEntity">The type of the block entity.</typeparam>
    /// <param name="world">The world to scan for blocks in.</param>
    /// <param name="origin">The origin position.</param>
    /// <param name="horRange">The horizontal (X/Z) range to scan.</param>
    /// <param name="vertRange">The vertical (Y) range to scan.</param>
    public static TBlockEntity GetNearestBlockEntity<TBlockEntity>(this IWorldAccessor world, BlockPos origin,
        float horRange, float vertRange) where TBlockEntity : BlockEntity
    {
        return world.GetNearestBlockEntity<TBlockEntity>(origin, horRange, vertRange, _ => true);
    }

    /// <summary>
    ///     Gets the nearest block, of a specified type, given an origin <see cref="BlockPos"/>.
    /// </summary>
    /// <typeparam name="TBlock">The type of the block entity.</typeparam>
    /// <param name="world">The world to scan for blocks in.</param>
    /// <param name="origin">The origin position.</param>
    /// <param name="horRange">The horizontal (X/Z) range to scan.</param>
    /// <param name="vertRange">The vertical (Y) range to scan.</param>
    /// <param name="predicate">A custom filter to narrow the focus of the search.</param>
    /// <param name="blockPosOut">The block position the block was found at.</param>
    public static TBlock GetNearestBlock<TBlock>(this IWorldAccessor world, BlockPos origin,
        float horRange, float vertRange, System.Func<TBlock, bool> predicate, out BlockPos blockPosOut) where TBlock : Block
    {
        TBlock blockEntity = null;
        BlockPos blockPosTemp = null;
        var found = false;
        var minPos = origin.AddCopy(-horRange, -vertRange, -horRange);
        var maxPos = origin.AddCopy(horRange, vertRange, horRange);
        world.BlockAccessor.WalkBlocks(minPos, maxPos, (block, x, y, z) =>
        {
            if (found) return;
            if (block.GetType() != typeof(TBlock) || !predicate((TBlock)block)) return;
            blockEntity = (TBlock)block;
            blockPosTemp = new BlockPos(x, y, z, Dimensions.NormalWorld);
            found = true;
        }, true);
        blockPosOut = blockPosTemp;
        return blockEntity;
    }

    /// <summary>
    ///     Gets the nearest block, of a specified type, given an origin <see cref="BlockPos"/>.
    /// </summary>
    /// <typeparam name="TBlock">The type of the block entity.</typeparam>
    /// <param name="world">The world to scan for blocks in.</param>
    /// <param name="origin">The origin position.</param>
    /// <param name="horRange">The horizontal (X/Z) range to scan.</param>
    /// <param name="vertRange">The vertical (Y) range to scan.</param>
    /// <param name="blockPosOut">The block position the block was found at.</param>
    public static TBlock GetNearestBlock<TBlock>(this IWorldAccessor world, BlockPos origin,
        float horRange, float vertRange, out BlockPos blockPosOut) where TBlock : Block
    {
        return world.GetNearestBlock<TBlock>(
            origin, horRange, vertRange, _ => true, out blockPosOut);
    }

    /// <summary>
    ///     Determines whether one <see cref="BlockPos"/> is within three-dimensional range of another <see cref="BlockPos"/>.
    /// </summary>
    /// <param name="sourceBlockPos">The position under test.</param>
    /// <param name="originBlockPos">The position used as the origin of the search.</param>
    /// <param name="horizontalRadius">The horizontal radius for the search.</param>
    /// <param name="verticalRadius">The vertical radius for the search.</param>
    /// <returns><c>true</c> if the source block position is within the specified range of the origin position, <c>false</c> otherwise.</returns>
    public static bool InRangeCubic(
        this BlockPos sourceBlockPos,
        BlockPos originBlockPos,
        int horizontalRadius = 10,
        int verticalRadius = 10)
    {
        if (!sourceBlockPos.InRangeHorizontally(originBlockPos.X, originBlockPos.Z, horizontalRadius)) return false;
        return sourceBlockPos.Y <= originBlockPos.Y + verticalRadius && sourceBlockPos.Y >= originBlockPos.Y - verticalRadius;
    }
}