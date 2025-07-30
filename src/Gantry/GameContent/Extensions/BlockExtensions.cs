namespace Gantry.GameContent.Extensions;

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
        => block?.Id == 0;

    /// <summary>
    ///     Determines whether the block should not be culled.
    /// </summary>
    /// <param name="block">The block to check.</param>
    public static bool IsNonCulled(this Block block) 
        => IsTypeNonCulled(block) || block.BlockBehaviors.Any(IsTypeNonCulled);

    /// <summary>
    ///     Determines whether the object should not be culled.
    /// </summary>
    /// <param name="obj">The object to check.</param>
    private static bool IsTypeNonCulled(object obj)
    {
        var objIsNonNullable = _nonCulledTypes.Contains(obj.GetType());
        var baseType = obj.GetType().BaseType;
        if (baseType is not null) objIsNonNullable |= _nonCulledTypes.Contains(baseType);
        return objIsNonNullable;
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