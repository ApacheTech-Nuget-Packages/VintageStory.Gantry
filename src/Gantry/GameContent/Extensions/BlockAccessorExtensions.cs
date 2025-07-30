using ApacheTech.Common.Extensions.Harmony;
using Vintagestory.API.MathTools;
using Vintagestory.Common;

namespace Gantry.GameContent.Extensions;

/// <summary>
///     Extends the functionality of blocks accessors within the game.
/// </summary>
public static class BlockAccessorExtensions
{
    /// <summary>
    ///     Determines whether any <see cref="BlockPos"/> in a list, is within three-dimensional range of another <see cref="BlockPos"/>.
    /// </summary>
    /// <param name="sourceBlockPositions">The positions under test.</param>
    /// <param name="originBlockPos">The position used as the origin of the search.</param>
    /// <param name="horizontalRadius">The horizontal radius for the search.</param>
    /// <param name="verticalRadius">The vertical radius for the search.</param>
    /// <returns><c>true</c> if the source block position is within the specified range of the origin position, <c>false</c> otherwise.</returns>
    public static bool AnyInRangeCubic(
        this IEnumerable<BlockPos> sourceBlockPositions,
        BlockPos originBlockPos,
        int horizontalRadius = 10,
        int verticalRadius = 10)
    {
        return (from sourceBlockPos in sourceBlockPositions
                let inRangeX =
                    sourceBlockPos.X <= originBlockPos.X + horizontalRadius &&
                    sourceBlockPos.X >= originBlockPos.X - horizontalRadius
                let inRangeY =
                    sourceBlockPos.Y <= originBlockPos.Y + verticalRadius &&
                    sourceBlockPos.Y >= originBlockPos.Y - verticalRadius
                let inRangeZ =
                    sourceBlockPos.Z <= originBlockPos.Z + horizontalRadius &&
                    sourceBlockPos.Z >= originBlockPos.Z - horizontalRadius
                where inRangeX && inRangeY && inRangeZ
                select inRangeX).Any();
    }

    /// <summary>
    ///     Searches blocks within a defined area, starting from the origin, and performs an action for each block found.
    /// </summary>
    /// <param name="blockAccessor">
    ///     The <see cref="IBlockAccessor"/> used to access blocks and chunks in the world.
    /// </param>
    /// <param name="minPos">
    ///     The minimum corner of the area to search, represented as a <see cref="BlockPos"/>.
    /// </param>
    /// <param name="maxPos">
    ///     The maximum corner of the area to search, represented as a <see cref="BlockPos"/>.
    /// </param>
    /// <param name="onBlock">
    ///     A consumable action invoked for each block in the search. The action is passed the block and its position.
    ///     Returning <c>false</c> from the action halts the search early.
    /// </param>
    /// <remarks>
    ///     This method ensures efficient block searches by loading only the necessary chunks into memory. 
    ///     Blocks are iterated in a 3D Morton order (Z-order curve) for better cache coherence.
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if <paramref name="onBlock"/> is <c>null</c>.
    /// </exception>
    public static async Task SearchBlocksFromOriginAsync(this IBlockAccessor blockAccessor, BlockPos minPos, BlockPos maxPos, ActionConsumable<Block, BlockPos> onBlock)
    {
        var worldMap = blockAccessor.GetField<WorldMap>("worldmap");
        var chunkSize = blockAccessor.GetField<int>("chunksize");
        var minx = GameMath.Clamp(Math.Min(minPos.X, maxPos.X), 0, worldMap.MapSizeX);
        var miny = GameMath.Clamp(Math.Min(minPos.Y, maxPos.Y), 0, worldMap.MapSizeY);
        var minz = GameMath.Clamp(Math.Min(minPos.Z, maxPos.Z), 0, worldMap.MapSizeZ);
        var maxx = GameMath.Clamp(Math.Max(minPos.X, maxPos.X), 0, worldMap.MapSizeX);
        var maxy = GameMath.Clamp(Math.Max(minPos.Y, maxPos.Y), 0, worldMap.MapSizeY);
        var maxz = GameMath.Clamp(Math.Max(minPos.Z, maxPos.Z), 0, worldMap.MapSizeZ);
        var mincx = minx / chunkSize;
        var mincy = miny / chunkSize;
        var mincz = minz / chunkSize;
        var maxcx = maxx / chunkSize;
        var maxcy = maxy / chunkSize;
        var maxcz = maxz / chunkSize;
        var chunks = blockAccessor.CallMethod<ChunkData[]>("LoadChunksToCache", mincx, mincy, mincz, maxcx, maxcy, maxcz, null);
        var cxCount = maxcx - mincx + 1;
        var czCount = maxcz - mincz + 1;

        var width = maxx - minx;
        var height = maxy - miny;
        var length = maxz - minz;
        var midX = width / 2;
        var midY = height / 2;
        var midZ = length / 2;
        for (var x = 0; x <= width; x++)
        {
            var px = x & 1;
            px = midX - (1 - px * 2) * (x + px) / 2;
            var posX = px + minx;
            var cix = posX / chunkSize - mincx;
            for (var y = 0; y <= height; y++)
            {
                var py = y & 1;
                py = midY - (1 - py * 2) * (y + py) / 2;
                var posY = py + miny;
                var index3dBase = posY % chunkSize * chunkSize * chunkSize + posX % chunkSize;
                var ciy = posY / chunkSize - mincy;
                for (var z = 0; z <= length; z++)
                {
                    var pz = z & 1;
                    pz = midZ - (1 - pz * 2) * (z + pz) / 2;
                    var posZ = pz + minz;
                    var ciz = posZ / chunkSize - mincz;
                    var chunkBlocks = chunks[(ciy * czCount + ciz) * cxCount + cix];
                    if (chunkBlocks is null) continue;
                    var index3d = index3dBase + posZ % chunkSize * chunkSize;
                    var blockId = chunkBlocks.GetFluid(index3d);
                    if (blockId == 0 || !worldMap.Blocks[blockId].SideSolid.Any)
                    {
                        blockId = chunkBlocks.GetSolidBlock(index3d);
                    }
                    if (!onBlock(worldMap.Blocks[blockId], new BlockPos(posX, posY, posZ, Dimensions.NormalWorld)))
                    {
                        await Task.CompletedTask;
                    }
                }
            }
        }
        await Task.CompletedTask;
    }
}