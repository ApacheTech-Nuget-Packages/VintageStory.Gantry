using Gantry.GameContent.GUI.Abstractions;

namespace Gantry.GameContent.Extensions.Gui;

/// <summary>
///     Extensions methods to aid the registration of GUI forms.
/// </summary>
public static class InputApiExtensions
{
    /// <summary>
    ///     Registers a hot key, and associates it with a dialogue form, as a singleton.
    /// </summary>
    public static void RegisterGuiDialogueHotKey(
        this IInputAPI api,
        GenericDialogue dialogue,
        string displayText,
        GlKeys hotKey,
        bool altPressed = false,
        bool ctrlPressed = false,
        bool shiftPressed = false)
    {
        api.RegisterHotKey(dialogue.ToggleKeyCombinationCode, displayText, hotKey, HotkeyType.GUIOrOtherControls, altPressed, ctrlPressed, shiftPressed);
        api.SetHotKeyHandler(dialogue.ToggleKeyCombinationCode, _ => dialogue.ToggleGui());
    }

    /// <summary>
    ///     Registers a hot key, and associates it with a dialogue form, as transient, using a factory to instantiate the dialogue.
    /// </summary>
    public static void RegisterTransientGuiDialogueHotKey<T>(
        this IInputAPI api,
        Func<T> dialogueFactory,
        string displayText,
        GlKeys hotKey,
        bool altPressed = false,
        bool ctrlPressed = false,
        bool shiftPressed = false)
        where T : GuiDialog
    {
        api.RegisterHotKey(nameof(T), displayText, hotKey, HotkeyType.GUIOrOtherControls, altPressed, ctrlPressed, shiftPressed);
        api.SetHotKeyHandler(nameof(T), _ => dialogueFactory().TryOpen());
    }
}