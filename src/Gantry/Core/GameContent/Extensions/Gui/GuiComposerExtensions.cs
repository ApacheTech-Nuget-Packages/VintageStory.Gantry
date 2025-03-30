using ApacheTech.Common.Extensions.Harmony;

namespace Gantry.Core.GameContent.Extensions.Gui;

/// <summary>
///     Extensions methods to aid the composition of GUI forms.
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class GuiComposerExtensions
{
    /// <summary>
    ///     The default bounds for a menu button.
    /// </summary>
    /// <returns>The default bounds for a menu button.</returns>

    public static ElementBounds DefaultButtonBounds()
    {
        return ElementBounds.Fixed(0.0, 0.0, 0.0, 40.0).WithFixedPadding(0.0, 3.0);
    }

    /// <summary>
    ///     Hides the GUI from view.
    /// </summary>
    public static GuiComposer ComposeHidden(this GuiComposer composer)
    {
        composer.Compose();
        composer.Bounds.Alignment = EnumDialogArea.None;
        composer.Bounds.fixedOffsetX = 0;
        composer.Bounds.fixedOffsetY = 0;
        composer.Bounds.absMarginX = 0;
        composer.Bounds.absMarginY = 0;
        return composer;
    }

    /// <summary>
    ///     The default bounds for a menu button.
    /// </summary>
    /// <returns>The default bounds for a menu button.</returns>

    public static ElementBounds DefaultButtonBounds(this GuiCompositeSettings _) => DefaultButtonBounds();

    /// <summary>
    ///     Returns all elements of a specific type, from the composer.
    /// </summary>
    public static List<T> GetElements<T>(this GuiComposer composer) where T : GuiElement 
        => [.. composer.GetElements().OfType<T>()];

    /// <summary>
    ///     Returns all elements that have been added to the composer.
    /// </summary>
    public static IEnumerable<GuiElement> GetElements(this GuiComposer composer)
    {
        var list = new List<GuiElement>();
        var interactiveElements = composer.GetField<Dictionary<string, GuiElement>>("interactiveElements");
        var staticElements = composer.GetField<Dictionary<string, GuiElement>>("staticElements");
        list.AddRange(interactiveElements.Values);
        list.AddRange(staticElements.Values);
        return list;
    }

    /// <summary>
    ///     Opens the GUI is closed, closes the GUI is open.
    /// </summary>
    /// <typeparam name="TDialogue">The type of the dialogue.</typeparam>
    /// <param name="dialogue">The dialogue.</param>
    /// <returns></returns>
    public static bool ToggleGui<TDialogue>(this TDialogue dialogue) where TDialogue : GuiDialog
    {
        return ApiEx.Client!.OpenedGuis.Contains(dialogue.ToggleKeyCombinationCode)
            ? dialogue.TryClose()
            : dialogue.TryOpen();
    }

    /// <summary>
    ///    Adds a lazy slider to the GUI composer, which triggers its action only on mouse release.
    /// </summary>
    /// <param name="composer">The GUI composer to which the slider is added.</param>
    /// <param name="onNewSliderValue">
    ///    A callback invoked with the slider's value when the slider is adjusted and the mouse is released.
    /// </param>
    /// <param name="bounds">The layout bounds of the slider element.</param>
    /// <param name="key">
    ///    An optional key to uniquely identify the slider element in the composer. Default is <c>null</c>.
    /// </param>
    /// <returns>
    ///    The updated <see cref="GuiComposer"/> instance to allow for method chaining.
    /// </returns>
    public static GuiComposer AddLazySlider(this GuiComposer composer, ActionConsumable<int> onNewSliderValue, ElementBounds bounds, string? key = null)
    {
        if (composer.Composed) return composer;
        var slider = new GuiElementSlider(composer.Api, onNewSliderValue, bounds);
        slider.CallMethod("TriggerOnlyOnMouseUp", true);
        composer.AddInteractiveElement(slider, key);
        return composer;
    }
}