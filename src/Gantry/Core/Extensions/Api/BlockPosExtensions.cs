using ApacheTech.Common.Extensions.Harmony;
using Gantry.Core.GameContent.Abstractions;
using Vintagestory.API.Common.Entities;
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

    /// <summary>
    ///     Converts a camera position to an entity position.
    /// </summary>
    /// <param name="cameraPoint">The camera position to convert.</param>
    public static EntityPos ToEntityPos(this CameraPoint cameraPoint)
    {
        return new()
        {
            X = cameraPoint.GetField<double>("x"),
            Y = cameraPoint.GetField<double>("y"),
            Z = cameraPoint.GetField<double>("z"),
            Pitch = cameraPoint.GetField<float>("pitch"),
            Yaw = cameraPoint.GetField<float>("yaw"),
            Roll = cameraPoint.GetField<float>("roll"),
        };
    }

    /// <summary>
    ///     Calculates the chunk position corresponding to the given block position.
    /// </summary>
    /// <param name="blockPos">The block position to convert to a chunk position.</param>
    /// <returns>The chunk position containing the specified block.</returns>
    /// <remarks>
    ///     This method divides the block position by <see cref="GlobalConstants.ChunkSize"/> 
    ///     to determine the chunk in which the block is located.
    /// </remarks>
    public static BlockPos ChunkPos(this BlockPos blockPos)
        => blockPos / GlobalConstants.ChunkSize;

}