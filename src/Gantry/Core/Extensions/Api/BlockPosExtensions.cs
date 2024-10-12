using Gantry.Core.GameContent.Abstractions;
using JetBrains.Annotations;
using Vintagestory.API.Common;
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
    /// <param name="api">The api to get the block accessor from.</param>
    /// <param name="layer">Each block pos can house a liquid, and a solid. Defaults to solid blocks.</param>
    public static TBlock GetBlock<TBlock>(this BlockPos @this, ICoreAPI api, BlockLayer layer = BlockLayer.Default)
        where TBlock : Block
    {
        return api.World.BlockAccessor.GetBlock(@this, (int)layer) as TBlock;
    }

    /// <summary>
    ///     Gets the block entity at the specified position within the gameworld.
    /// </summary>
    /// <typeparam name="TBlockEntity">The type of the block.</typeparam>
    /// <param name="this">The <see cref="BlockPos"/> to find the block at.</param>
    /// <param name="api">The api to get the block accessor from.</param>
    public static TBlockEntity GetBlockEntity<TBlockEntity>(this BlockPos @this, ICoreAPI api)
        where TBlockEntity : BlockEntity
    {
        return api.World.BlockAccessor.GetBlockEntity(@this) as TBlockEntity;
    }
}