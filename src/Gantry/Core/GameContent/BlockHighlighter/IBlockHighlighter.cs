using Cairo;
using Vintagestory.API.MathTools;

namespace Gantry.Core.GameContent.BlockHighlighter;

/// <summary>
///     Allows blocks, or selected areas to be highlighted with a specific colour.
/// </summary>
public interface IBlockHighlighter
{
    /// <summary>
    ///     Adds a coloured highlight pointer to the cache within the highlighter.
    /// </summary>
    /// <param name="colour">The colour to highlight blocks with.</param>
    /// <returns>An <see cref="int"/> that acts as the identifier for this highlight pointer.</returns>
    int AddHighlight(Color colour);

    /// <summary>
    ///     Highlights the specified block, using a given highlight pointer to gather colour information from.
    /// </summary>
    /// <param name="index">The index within the cache, assigned by <see cref="AddHighlight"/>.</param>
    /// <param name="block">The block to highlight.</param>
    void Highlight(int index, BlockPos block);

    /// <summary>
    ///     Highlights the specified block, using a given highlight pointer to gather colour information from.
    /// </summary>
    /// <param name="index">The index within the cache, assigned by <see cref="AddHighlight"/>.</param>
    /// <param name="blocks">The blocks to highlight.</param>
    void Highlight(int index, IEnumerable<BlockPos> blocks);

    /// <summary>
    ///     Highlights the specified range of blocks, using a given highlight pointer to gather colour information from.
    /// </summary>
    /// <param name="index">The index within the cache, assigned by <see cref="AddHighlight"/>.</param>
    /// <param name="area">The blocks to highlight.</param>
    void HighlightArea(int index, Cuboidi area);

    /// <summary>
    ///     Clears the highlighting from a specific highlight pointer.
    /// </summary>
    /// <param name="index">The index within the cache, to clear.</param>
    void ClearHighlighting(int index);
}