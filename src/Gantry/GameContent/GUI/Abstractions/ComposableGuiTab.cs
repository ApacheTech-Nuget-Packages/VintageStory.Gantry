namespace Gantry.GameContent.GUI.Abstractions;

/// <summary>
///     Base class for GUI tabs that can be composed into a `GenericDialogue`.
///     Provides common fonts and layout helpers for derived composable parts.
/// </summary>
public abstract class ComposableGuiTab : GuiTab, IGuiComposablePart
{
    /// <summary>
    ///     Default width, in pixels, used when displaying hover text.
    /// </summary>
    protected const int HOVER_TEXT_WIDTH = 260;

    /// <summary>
    ///     The layout bounds occupied by this composable part within the parent dialogue.
    /// </summary>
    public abstract ElementBounds Bounds { get; set; }

    /// <summary>
    ///     Uses the supplied <see cref="GuiComposer"/> to compose this part into the parent dialogue.
    /// </summary>
    /// <param name="parent">The parent <see cref="GenericDialogue"/> instance this part is being composed into.</param>
    /// <param name="composer">The <see cref="GuiComposer"/> used to place and configure UI elements.</param>
    /// <returns>The <see cref="GuiComposer"/> instance passed in, after composing this part.</returns>
    public abstract GuiComposer ComposePart(GenericDialogue parent, GuiComposer composer);

    /// <summary>
    ///     Refreshes the displayed values for the controls that belong to this part.
    /// </summary>
    /// <param name="composer">The <see cref="GuiComposer"/> that contains this part's controls.</param>
    public abstract void RefreshValues(GuiComposer composer);

    /// <summary>
    ///     Font used for labels within the tab.
    /// </summary>
    protected CairoFont LabelFont { get; } = CairoFont.WhiteSmallText();

    /// <summary>
    ///     Font used for hover and detail text within the tab.
    /// </summary>
    protected CairoFont HoverTextFont { get; } = CairoFont.WhiteDetailText();

    /// <summary>
    ///     Helper method that creates two aligned row bounds: one for the left element (typically a label)
    ///     and one for the right element (typically a control). The left bound is placed under
    ///     <paramref name="leftUnder"/> and the right bound is placed under <paramref name="rightUnder"/>.
    /// </summary>
    /// <param name="leftUnder">The bounds to place the left element under.</param>
    /// <param name="rightUnder">The bounds to place the right element under.</param>
    /// <param name="left">Output parameter that receives the calculated left bounds.</param>
    /// <param name="right">Output parameter that receives the calculated right bounds.</param>
    protected void SetRowBounds(ElementBounds leftUnder, ElementBounds rightUnder, out ElementBounds left, out ElementBounds right)
    {
        var leftPad = leftUnder == Bounds ? 15 : 10;
        left = ElementBounds.FixedSize(250, 30).FixedUnder(leftUnder, leftPad);
        right = ElementBounds.FixedSize(320, 30).FixedUnder(rightUnder, 10).FixedRightOf(left, 10);
    }
}