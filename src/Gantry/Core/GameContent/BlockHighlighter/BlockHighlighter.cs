using Cairo;
using Gantry.Core.Hosting.Annotation;
using Vintagestory.API.MathTools;

namespace Gantry.Core.GameContent.BlockHighlighter;

/// <summary>
///     Allows blocks, or selected areas to be highlighted with a specific colour.
/// </summary>
[UsedImplicitly]
public class BlockHighlighter : IBlockHighlighter
{
    private readonly ICoreClientAPI _capi;
    private readonly Dictionary<int, Color> _cache = [];

    /// <summary>
    ///     Initialises a new instance of the <see cref="BlockHighlighter"/> class.
    /// </summary>
    /// <param name="capi">The Client-side Api. Proffered through IOC.</param>
    [SidedConstructor(EnumAppSide.Client)]
    public BlockHighlighter(ICoreClientAPI capi) => _capi = capi;

    /// <inheritdoc/>
    public int AddHighlight(Color colour)
    {
        var index = 0;
        while (_cache.ContainsKey(index)) index++;
        _cache.Add(index, colour);
        return index;
    }

    /// <inheritdoc/>
    public void Highlight(int index, BlockPos block)
    {
        Highlight(index, [block]);
    }

    /// <inheritdoc/>
    public void Highlight(int index, IEnumerable<BlockPos> blocks)
    {
        var positions = blocks.ToList();
        var colour = _cache[index];
        var colours = positions.Select(_ => ColorUtil.FromRGBADoubles([colour.R, colour.G, colour.B, colour.A])).ToList();
        _capi.World.HighlightBlocks(
            _capi.World.Player,
            index,
            positions,
            colours);
    }

    /// <inheritdoc/>
    public void HighlightArea(int index, Cuboidi area)
    {
        var colour = _cache[index];
        var blocks = new List<BlockPos> { area.LowerBounds(), area.ExclusiveUpperBounds() };
        var colours = new List<int> { ColorUtil.FromRGBADoubles([colour.R, colour.G, colour.B, colour.A]) };

        ApiEx.Client.World.HighlightBlocks(
            ApiEx.Client.World.Player,
            index,
            blocks,
            colours,
            EnumHighlightBlocksMode.Absolute,
            EnumHighlightShape.Cubes);
    }

    /// <inheritdoc/>
    public void ClearHighlighting(int index)
        => _capi.World.HighlightBlocks(_capi.World.Player, index, []);
}