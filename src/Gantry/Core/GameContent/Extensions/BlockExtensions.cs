using Vintagestory.API.Common;

using Vintagestory.GameContent;

namespace Gantry.Core.GameContent.Extensions;

/// <summary>
///     Extends the functionality of blocks within the game.
/// </summary>
public static class BlockExtensions
{
    /// <summary>
    ///     Determines whether the block is an air block.
    /// </summary>
    /// <param name="block">The block to check.</param>
    public static bool IsAirBlock(this Block block)
    {
        block ??= ApiEx.Current.World.GetBlock(0);
        return block.Id == 0;
    }

    /// <summary>
    ///     Determines whether the block should not be culled.
    /// </summary>
    /// <param name="block">The block to check.</param>
    public static bool IsNonCulled(this Block block)
    {
        return IsTypeNonCulled(block) || block.BlockBehaviors.Any(IsTypeNonCulled);
    }

    /// <summary>
    ///     Determines whether the object should not be culled.
    /// </summary>
    /// <param name="obj">The object to check.</param>
    private static bool IsTypeNonCulled(object obj)
    {
        return _nonCulledTypes.Contains(obj.GetType()) || _nonCulledTypes.Contains(obj.GetType().BaseType);
    }

    private static readonly List<Type> _nonCulledTypes =
    [
        typeof(BlockFernTree),
        typeof(BlockPlant),
        typeof(BlockVines),
        typeof(BlockLeaves),
        typeof(BlockSeaweed)
    ];
}