using System;
using JetBrains.Annotations;
using Vintagestory.API.Common;

namespace Gantry.Core.GameContent.Blocks
{
    /// <summary>
    ///     Basic class for block entities - a data structures to hold custom
    ///     information for blocks, e.g. for chests to hold its contents.
    /// </summary>
    /// <typeparam name="TBlockEntity">The type of the block entity.</typeparam>
    /// <seealso cref="BlockEntityBehavior" />
    [UsedImplicitly(ImplicitUseTargetFlags.All)]
    public abstract class BlockEntityBehaviour<TBlockEntity> : BlockEntityBehavior where TBlockEntity: BlockEntity
    {
        /// <summary>
        ///     The <see cref="BlockEntity"/> this behaviour is applied to.
        /// </summary>
        protected TBlockEntity Entity { get; }

        /// <summary>
        ///     Initialises a new instance of the <see cref="BlockEntityBehaviour{TBlockEntity}"/> class.
        /// </summary>
        /// <param name="blockEntity">The <see cref="BlockEntity"/> this behaviour is applied to.</param>
        /// <exception cref="InvalidCastException">This behaviour cannot be applied to the specified block entity.</exception>
        protected BlockEntityBehaviour(BlockEntity blockEntity) : base(blockEntity)
        {
            if (blockEntity is not TBlockEntity entity)
            {
                throw new InvalidCastException("This behaviour cannot be applied to the specified block entity.");
            }
            Entity = entity;
        }
    }
}