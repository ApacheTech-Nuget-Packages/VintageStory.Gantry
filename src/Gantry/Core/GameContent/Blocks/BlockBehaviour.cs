namespace Gantry.Core.GameContent.Blocks;

/// <summary>
///     A behaviour that can be added to a block, to allow it to interact, or react, to external stimuli; or maintain an internal state.
/// </summary>
/// <typeparam name="TBlock">The type of the block.</typeparam>
/// <seealso cref="BlockBehavior" />
[UsedImplicitly(ImplicitUseTargetFlags.All)]
public abstract class BlockBehaviour<TBlock> : BlockBehavior where TBlock : Block
{
    /// <summary>
    ///     The block to apply this behaviour to.
    /// </summary>
    protected TBlock BlockInstance { get; }

    /// <summary>
    ///     Initialises a new instance of the <see cref="BlockBehaviour{TBlock}"/> class.
    /// </summary>
    /// <param name="block">The block to apply this behaviour to.</param>
    /// <exception cref="InvalidCastException">This behaviour cannot be applied to the specified block.</exception>
    protected BlockBehaviour(Block block) : base(block)
    {
        if (block is not TBlock blockInstance)
        {
            throw new InvalidCastException("This behaviour cannot be applied to the specified block.");
        }
        BlockInstance = blockInstance;
    }
}