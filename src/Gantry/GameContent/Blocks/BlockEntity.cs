namespace Gantry.GameContent.Blocks;

/// <summary>
///     Basic class for block entities - a data structures to hold custom
///     information for blocks, e.g. for chests to hold its contents.
/// </summary>
/// <typeparam name="TBlock">The type of the block.</typeparam>
/// <seealso cref="BlockEntity" />
public class BlockEntity<TBlock> : BlockEntity where TBlock : Block
{
    /// <summary>
    ///     The black that this entity applies to.
    /// </summary>
    protected TBlock? OwnerBlock => Block as TBlock;
}