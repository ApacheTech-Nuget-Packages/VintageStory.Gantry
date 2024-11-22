using JetBrains.Annotations;
using Vintagestory.API.Client;

namespace Gantry.Core.GameContent.Extensions.Gui;

/// <summary>
///     Extensions methods to aid the registration of GUI forms.
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class InputApiExtensions
{
    /// <summary>
    ///     Registers a hot key, and associates it with a dialogue form, as a singleton.
    /// </summary>
    public static void RegisterGuiDialogueHotKey(
        this IInputAPI api,
        GuiDialog dialogue,
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
    public static void RegisterTransientGuiDialogueHotKey(
        this IInputAPI api,
        Func<GuiDialog> dialogueFactory,
        string displayText,
        GlKeys hotKey,
        bool altPressed = false,
        bool ctrlPressed = false,
        bool shiftPressed = false)
    {
        var dialogue = dialogueFactory();
        api.RegisterHotKey(dialogue.ToggleKeyCombinationCode, displayText, hotKey, HotkeyType.GUIOrOtherControls, altPressed, ctrlPressed, shiftPressed);
        api.SetHotKeyHandler(dialogue.ToggleKeyCombinationCode, _ => dialogueFactory().TryOpen());
    }
}