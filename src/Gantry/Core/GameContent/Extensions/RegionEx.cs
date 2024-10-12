using Vintagestory.API.MathTools;

namespace Gantry.Core.GameContent.Extensions;

/// <summary>
///     Extended functionality for positional regions.
/// </summary>
public static class RegionEx
{
    /// <summary>
    ///     The exclusive lower bounds of a region.
    /// </summary>
    /// <param name="startPos">The start position.</param>
    /// <param name="endPos">The end position.</param>
    /// <returns>The block position outside the lower bounds of the selected region.</returns>
    public static BlockPos ExclusiveLowerBounds(BlockPos startPos, BlockPos endPos)
        => new Cuboidi(startPos, endPos).ExclusiveLowerBounds();

    /// <summary>
    ///     The inclusive lower bounds of a region.
    /// </summary>
    /// <param name="startPos">The start position.</param>
    /// <param name="endPos">The end position.</param>
    /// <returns>The block position inside the lower bounds of the selected region.</returns>
    public static BlockPos LowerBounds(BlockPos startPos, BlockPos endPos)
        => new Cuboidi(startPos, endPos).LowerBounds();

    /// <summary>
    ///     The inclusive upper bounds of a region.
    /// </summary>
    /// <param name="startPos">The start position.</param>
    /// <param name="endPos">The end position.</param>
    /// <returns>The block position inside the upper bounds of the selected region.</returns>
    public static BlockPos UpperBounds(BlockPos startPos, BlockPos endPos)
        => new Cuboidi(startPos, endPos).UpperBounds();

    /// <summary>
    ///     The exclusive upper bounds of a region.
    /// </summary>
    /// <param name="startPos">The start position.</param>
    /// <param name="endPos">The end position.</param>
    /// <returns>The block position outside the upper bounds of the selected region.</returns>
    public static BlockPos ExclusiveUpperBounds(BlockPos startPos, BlockPos endPos)
        => new Cuboidi(startPos, endPos).ExclusiveUpperBounds();
}