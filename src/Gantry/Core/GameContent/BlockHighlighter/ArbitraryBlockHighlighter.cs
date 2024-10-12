using ApacheTech.Common.Extensions.System;
using Cairo;
using Vintagestory.API.MathTools;

namespace Gantry.Core.GameContent.BlockHighlighter;

/// <summary>
///     Allows blocks to be arbitrarily highlighted in a specific colour.
/// </summary>
public class ArbitraryBlockHighlighter
{
    private readonly IBlockHighlighter _highlighter;
    private int? _id = null;
    private readonly List<BlockPos> _positions = [];

    /// <summary>
    ///     Initialises a new instance of the <see cref="ArbitraryBlockHighlighter"/> class.
    /// </summary>
    public ArbitraryBlockHighlighter(IBlockHighlighter highlighter)
    {
        _highlighter = highlighter;
    }

    /// <summary>
    ///     Sets the colour of the highlighted areas.
    /// </summary>
    /// <param name="colour">The colour to use render the block highlights.</param>
    public void SetColour(Color colour)
    {
        _id = _highlighter.AddHighlight(colour);
    }

    /// <summary>
    ///     Clear the highlights from the blocks.
    /// </summary>
    public void Clear()
    {
        if (_id is null) throw new InvalidOperationException("No colour has been set for the highlighter.");
        _highlighter.ClearHighlighting(_id.Value);
        _positions.Clear();
    }

    /// <summary>
    ///     Add a position to highlight.
    /// </summary>
    /// <param name="position"></param>
    public void AddPosition(BlockPos position)
        => _positions.AddIfNotPresent(position);

    /// <summary>
    ///     Highlight the selected blocks.
    /// </summary>
    public void Highlight()
    {
        if (_id is null) throw new InvalidOperationException("No colour has been set for the highlighter.");
        _highlighter.Highlight(_id.Value, _positions);
    }
}