using ApacheTech.Common.Extensions.Harmony;
using ApacheTech.Common.Mediator.Commands.Handlers;
using Gantry.Extensions;
using Gantry.Extensions.Api;
using Gantry.GameContent;
using Gantry.GameContent.Extensions;
using Vintagestory.API.MathTools;

namespace Gantry.Utilities.BlockScanner;

/// <summary />
public class BlockScannerHandler : CommandHandlerBase<BlockScannerCommand>
{
    private readonly ICoreGantryAPI _gantry;

    /// <summary />
    public BlockScannerHandler(ICoreGantryAPI gantry)
    {
        _gantry = gantry;
    }

    /// <summary />
    public override async Task HandleAsync(BlockScannerCommand command, CancellationToken cancellationToken)
    {
        var code = command.Code.ToLowerInvariant();
        var origin = command.Origin;
        var horizontalRadius = command.HorizontalRadius;
        var verticalRadius = command.VerticalRadius;
        var matchDisplayedName = command.MatchDisplayedName;
        var firstOnly = command.FirstOnly;

        var worldHeight = _gantry.Capi.ClientMain.GetField<ClientWorldMap>("WorldMap")!.MapSizeY;
        var minPos = new BlockPos(origin.X - horizontalRadius, Math.Max(origin.Y - verticalRadius, 1), origin.Z - horizontalRadius, Dimensions.NormalWorld);
        var maxPos = new BlockPos(origin.X + horizontalRadius, Math.Min(origin.Y + verticalRadius, worldHeight), origin.Z + horizontalRadius, Dimensions.NormalWorld);

        var walker = _gantry.Capi.World.GetBlockAccessor(synchronize: false, relight: false, strict: false);

        try
        {
            await walker.SearchBlocksFromOriginAsync(minPos, maxPos, OnBlock);
        }
        catch (NullReferenceException)
        {
            _gantry.Capi.EnqueueShowChatMessage("Ran into unloaded chunks. Try a smaller radius, or generate all chunks within range.");
        }

        bool OnBlock(Block block, BlockPos blockPos)
        {
            if (firstOnly && command.FoundBlocks.Count > 0) return false;
            if (command.FoundBlocks.Any(kvp => kvp.Position.Equals(blockPos))) return true;
            if (block.BlockMaterial is EnumBlockMaterial.Air or EnumBlockMaterial.Water) return true;
            if (command.FoundBlocks.Select(kvp => kvp.Position).AnyInRangeCubic(blockPos, 15, 256)) return true;

            switch (matchDisplayedName)
            {
                case false when !block.Code.ToString().Contains(code, StringComparison.InvariantCultureIgnoreCase):
                case true when !block.GetPlacedBlockName(_gantry.Capi.ClientMain, blockPos).Contains(code, StringComparison.InvariantCultureIgnoreCase):
                    return true;
                default:
                    _gantry.Capi.EnqueueShowChatMessage($"Found At: {blockPos}");
                    command.FoundBlocks.Add(new(blockPos.Copy(), block));
                    return true;
            }
        }
    }
}