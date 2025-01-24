using Vintagestory.API.MathTools;

namespace Gantry.Core.GameContent.Extensions;

/// <summary>
///     Extends the functionality of cuboids within the game.
/// </summary>
public static class CuboidExtensions
{
    /// <summary>
    ///     Performs an transformative action on all block positions within a specific area.
    /// </summary>
    /// <typeparam name="T">The type expected as an output from all the blocks within the area.</typeparam>
    /// <param name="this">The area to perform the action on.</param>
    /// <param name="f">The action to perform.</param>
    public static IEnumerable<T> MapAll<T>(this Cuboidi @this, System.Func<BlockPos, T> f)
    {
        for (var y = @this.MinY; y <= @this.MaxY; y++)
            for (var x = @this.MinX; x <= @this.MaxX; x++)
                for (var z = @this.MinZ; z <= @this.MaxZ; z++)
                    yield return f(new BlockPos(x, y, z, 0));
    }

    /// <summary>
    ///     Performs an action on all block positions within a specific area.
    /// </summary>
    /// <param name="this">The area to perform the action on.</param>
    /// <param name="f">The action to perform.</param>
    public static void InvokeAll(this Cuboidi @this, Action<BlockPos> f)
    {
        for (var y = @this.MinY; y <= @this.MaxY; y++)
            for (var x = @this.MinX; x <= @this.MaxX; x++)
                for (var z = @this.MinZ; z <= @this.MaxZ; z++)
                    f(new BlockPos(x, y, z, 0));
    }

    /// <summary>
    ///     Gets a specific block position within an area, relative to the whole area.
    /// </summary>
    /// <param name="this">The area.</param>
    /// <param name="relativePosition">A specific position within the area to get the block position of.</param>
    public static BlockPos GetPosition(this Cuboidi @this, Vec3i relativePosition)
        => @this.LowerBounds().AddCopy(relativePosition);

    /// <summary>
    ///     Gets a specific block position within an area, relative to the whole area.
    /// </summary>
    /// <param name="this">The area.</param>
    /// <param name="index">A specific position within the area to get the block position of.</param>
    public static BlockPos GetPosition(this Cuboidi @this, int index)
    {
        var z = index;
        var y = z / @this.SizeXZ;
        z -= y * @this.SizeXZ;
        var x = z / @this.SizeZ;
        z -= x * @this.SizeZ;

        return @this.GetPosition(new Vec3i(x, y, z));
    }

    /// <summary>
    ///     Converts a cuboid into a list of block positions.
    /// </summary>
    /// <param name="cuboid">The cuboid to flatten.</param>
    public static IEnumerable<BlockPos> Flatten(this Cuboidi cuboid)
    {
        for (var y = cuboid.MinY; y <= cuboid.MaxY; y++)
            for (var x = cuboid.MinX; x <= cuboid.MaxX; x++)
                for (var z = cuboid.MinZ; z <= cuboid.MaxZ; z++)
                    yield return new BlockPos(x, y, z, 0);
    }

    /// <summary>
    ///     The exclusive lower bounds of an area.
    /// </summary>
    /// <param name="cuboid">The area.</param>
    /// <returns>The block position outside the lower bounds of the selected region.</returns>
    public static BlockPos ExclusiveLowerBounds(this Cuboidi cuboid)
        => new(cuboid.MinX - 1, cuboid.MinY - 1, cuboid.MinZ - 1, 0);

    /// <summary>
    ///     The inclusive lower bounds of an area.
    /// </summary>
    /// <param name="cuboid">The area.</param>
    /// <returns>The block position inside the lower bounds of the selected region.</returns>
    public static BlockPos LowerBounds(this Cuboidi cuboid)
        => new(cuboid.MinX, cuboid.MinY, cuboid.MinZ, 0);

    /// <summary>
    ///     The inclusive upper bounds of an area.
    /// </summary>
    /// <param name="cuboid">The area.</param>
    /// <returns>The block position inside the upper bounds of the selected region.</returns>
    public static BlockPos UpperBounds(this Cuboidi cuboid)
        => new(cuboid.MaxX, cuboid.MaxY, cuboid.MaxZ, 0);

    /// <summary>
    ///     The exclusive upper bounds of an area.
    /// </summary>
    /// <param name="cuboid">The area.</param>
    /// <returns>The block position outside the upper bounds of the selected region.</returns>
    public static BlockPos ExclusiveUpperBounds(this Cuboidi cuboid)
        => new(cuboid.MaxX + 1, cuboid.MaxY + 1, cuboid.MaxZ + 1, 0);
}