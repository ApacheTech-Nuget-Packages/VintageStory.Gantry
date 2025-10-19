using Vintagestory.API.MathTools;

namespace Gantry.Extensions.Api;

/// <summary>
///     Provides extension methods for working with world chunks related to the client and block positions.
///     These helpers operate on client API and block position types to produce chunk coordinates.
/// </summary>
public static class ChunkExtensions
{
    /// <summary>
    ///     Returns a set of chunk coordinates within the specified render distance of the current player.
    ///     The player's block position is converted into a chunk coordinate and all chunks within a square
    ///     range (inclusive) around that chunk are returned.
    /// </summary>
    /// <param name="capi">The client API instance.</param>
    /// <param name="renderDistance">Range in chunks from the player's chunk (inclusive).</param>
    /// <returns>A <see cref="HashSet{FastVec2i}"/> containing distinct chunk coordinates within range.</returns>
    public static HashSet<FastVec2i> GetChunksInRangeOfPlayer(this ICoreClientAPI capi, int renderDistance)
    {
        var playerPos = capi.World.Player.Entity.Pos.AsBlockPos;
        return [.. playerPos.GetChunksInRange(renderDistance).Distinct()];
    }

    /// <summary>
    ///     Enumerates chunk coordinates within a square range centred on the chunk containing the given block position.
    ///     The enumeration yields chunk coordinates row by row from negative to positive offsets.
    /// </summary>
    /// <param name="pos">Block position used to determine the central chunk.</param>
    /// <param name="rangeInChunks">Number of chunks to include in each direction from the centre chunk.</param>
    /// <returns>An enumerable of <see cref="FastVec2i"/> representing chunk coordinates.</returns>
    public static IEnumerable<FastVec2i> GetChunksInRange(this BlockPos pos, int rangeInChunks)
    {
        var chunk = pos.Chunk();
        for (var dx = -rangeInChunks; dx <= rangeInChunks; dx++)
            for (var dz = -rangeInChunks; dz <= rangeInChunks; dz++)
                yield return new(chunk.X + dx, chunk.Y + dz);
    }
}