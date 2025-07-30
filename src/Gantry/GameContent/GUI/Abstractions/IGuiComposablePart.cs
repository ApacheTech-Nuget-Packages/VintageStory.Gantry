namespace Gantry.GameContent.GUI.Abstractions;

/// <summary>
///     Represents a composable part, that can be rendered to a GUI dialogue.
/// </summary>
public interface IGuiComposablePart
{
    /// <summary>
    ///     Uses the injected <see cref="GuiComposer"/> to compose part of a  dialogue window.
    /// </summary>
    /// <returns></returns>
    GuiComposer ComposePart(GuiComposer composer);

    /// <summary>
    ///     Refreshes the displayed values on the form.
    /// </summary>
    void RefreshValues(GuiComposer composer);

    /// <summary>
    ///     The bounds of the element.
    /// </summary>
    ElementBounds Bounds { get; set; }
}