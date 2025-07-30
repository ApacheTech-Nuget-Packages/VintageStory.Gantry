using ApacheTech.Common.Extensions.Harmony;
using Cairo;
using Gantry.GameContent.GUI.Abstractions;
using Gantry.GameContent.GUI.Elements;
using System.Text;
using GuiElementImage = Gantry.GameContent.GUI.Elements.GuiElementImage;

namespace Gantry.GameContent.GUI.Helpers;

/// <summary>
///     Extension methods to aid when composing GUIs.
/// </summary>
public static class GuiComposerHelpers
{
    /// <summary>
    ///     Adds a composable part to a GUI composer.
    /// </summary>
    /// <param name="composer">The composer.</param>
    /// <param name="content">The content.</param>
    /// <returns></returns>
    public static GuiComposer AddComposablePart(this GuiComposer composer, IGuiComposablePart content)
        => content.ComposePart(composer);

    /// <summary>
    ///     Adds a dialogue title bar to the GUI, with no "Movable" menu bar.  
    /// </summary>
    /// <param name="composer"></param>
    /// <param name="text">The text of the title bar.</param>
    /// <param name="onClose">The event fired when the title bar is closed.</param>
    /// <param name="font">The font of the title bar.</param>
    /// <param name="bounds">The bounds of the title bar.</param>
    public static GuiComposer AddTitleBarWithNoMenu(this GuiComposer composer, string text, Action? onClose = null, CairoFont? font = null, ElementBounds? bounds = null)
    {
        if (!composer.Composed)
        {
            composer.AddInteractiveElement(new GuiElementTitleBar(composer.Api, text, onClose, font, bounds));
        }
        return composer;
    }

    /// <summary>
    ///     Adds a PNG image to the composer, scaled to the dimensions of the containing <see cref="ElementBounds"/>. 
    /// </summary>
    /// <param name="composer">The composer to add the image to.</param>
    /// <param name="imageAsset">The image asset to add.</param>
    /// <param name="bounds">The containing <see cref="ElementBounds"/>. This defines the dimensions of the image to load.</param>
    /// <param name="blendMode">The mode with which to . Defaults to <see cref="Operator.Over"/>.</param>
    /// <returns>GuiComposer.</returns>
    public static GuiComposer AddStaticImage(this GuiComposer composer, AssetLocation imageAsset, ElementBounds bounds, Operator blendMode = Operator.Over)
    {
        if (!composer.Composed)
        {
            composer.AddStaticElement(new GuiElementImage(composer.Api, bounds, imageAsset, blendMode));
        }
        return composer;
    }

    /// <summary>
    ///     Adds hover-text for each cell within a cell list, based on the content of its `HoverText` property.
    /// </summary>
    /// <typeparam name="TCellEntry">The type of cell entry the list is populated with.</typeparam>
    /// <param name="composer">The composer the cell list belongs to.</param>
    /// <param name="cellListName">The name, given to the cell list, to identify it within the composer.</param>
    /// <param name="clippedBounds">The bounds to clip the hover-text elements to.</param>
    /// <returns>The same instance of <see cref="GuiComposer"/> that this extension method was called on.</returns>
    /// <remarks>
    ///     Ripped from the main menu of the game, and genericised, to form an extension method.
    ///     Originally, this code was made to create hover-text for save-game entries.
    /// </remarks>
    public static GuiComposer AddHoverTextForCellList<TCellEntry>(this GuiComposer composer, string cellListName, ElementBounds clippedBounds)
        where TCellEntry : SavegameCellEntry
    {
        var cellListElement = composer.GetCellList<TCellEntry>(cellListName);
        var cellEntries = cellListElement.GetField<List<TCellEntry>>("cellsTmp");
        cellListElement.BeforeCalcBounds();
        for (var i = 0; i < cellEntries.Count; i++)
        {
            var hoverText = new StringBuilder(cellEntries[i].Title);
            hoverText.AppendLine(cellEntries[i].HoverText);

            var hoverTextBounds = cellListElement.elementCells[i].Bounds.ForkChild();

            cellListElement.elementCells[i].Bounds.ChildBounds.Add(hoverTextBounds);
            hoverTextBounds.fixedWidth -= 56.0;
            hoverTextBounds.fixedY = -3.0;
            hoverTextBounds.fixedX -= 6.0;
            hoverTextBounds.fixedHeight -= 2.0;
            composer.AddHoverText(hoverText.ToString(), CairoFont.WhiteDetailText(), 320, hoverTextBounds, $"{cellListName}-hover-{i}");
            composer.GetHoverText($"{cellListName}-hover-{i}").InsideClipBounds = clippedBounds;
        }

        return composer;
    }

    /// <summary>
    ///     Only triggers the callback methods when the user releases the mouse.
    /// </summary>
    /// <param name="slider">The slider.</param>
    /// <param name="value">if set to <c>true</c> the callback action will only be called when the user releases the mouse.</param>
    public static void TriggerOnlyOnMouseUp(this GuiElementSlider slider, bool value)
    {
        slider.CallMethod("TriggerOnlyOnMouseUp", value);
    }

    /// <summary>
    ///     Composes the hover-text element associated with the slider.
    /// </summary>
    /// <param name="slider">The slider.</param>
    public static void ComposeHoverTextElement(this GuiElementSlider slider)
    {
        slider.CallMethod("ComposeHoverTextElement");
    }
}