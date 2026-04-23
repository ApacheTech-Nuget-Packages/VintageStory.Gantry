using Gantry.GameContent;
using Vintagestory.API.MathTools;

namespace Gantry.Utilities.BlockScanner;

/// <summary />
public class BlockScannerCommand : CommandBase
{
    /// <summary />
    public required string Code { get; set; }

    /// <summary />
    public required BlockPos Origin { get; set; }

    /// <summary />
    public int HorizontalRadius { get; set; } = 100;

    /// <summary />
    public int VerticalRadius { get; set; } = 50;

    /// <summary />
    public bool MatchDisplayedName { get; set; }

    /// <summary />
    public bool FirstOnly { get; set; }

    /// <summary />
    public List<BlockWithPos> FoundBlocks { get; private set; } = [];

    /// <summary />
    public BlockWithPos? ClosestBlock => FoundBlocks.FirstOrDefault();

    /// <summary />
    public bool HasFoundBlocks => FoundBlocks.Count > 0;
}
