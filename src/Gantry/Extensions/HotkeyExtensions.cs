namespace Gantry.Extensions;

/// <summary>
///     Extension methods to aid working with KeyCombination objects.
/// </summary>
public static class HotkeyExtensions
{
    /// <summary>
    ///     Converts a specified key combination to a corresponding key event representation.
    /// </summary>
    /// <param name="keyCombination">The key combination to convert to a key event.</param>
    /// <returns>A new KeyEvent instance that represents the specified key combination.</returns>
    public static KeyEvent ToKeyEvent(this KeyCombination keyCombination) => new()
    {
        KeyCode = keyCombination.KeyCode,
        KeyCode2 = keyCombination.SecondKeyCode,
        CtrlPressed = keyCombination.Ctrl,
        AltPressed = keyCombination.Alt,
        ShiftPressed = keyCombination.Shift
    };

    /// <summary>
    ///     Creates a new KeyCombination instance that represents the key codes and modifier states from the specified KeyEvent.
    /// </summary>
    /// <param name="keyEvent">The KeyEvent containing the key codes and modifier key states to convert.</param>
    /// <returns>A KeyCombination that reflects the key codes and modifier keys from the provided KeyEvent.</returns>
    public static KeyCombination ToKeyCombination(this KeyEvent keyEvent) => new()
    {
        KeyCode = keyEvent.KeyCode,
        SecondKeyCode = keyEvent.KeyCode2,
        Ctrl = keyEvent.CtrlPressed,
        Alt = keyEvent.AltPressed,
        Shift = keyEvent.ShiftPressed
    };

    /// <summary>
    ///     Determines whether the specified key event matches the key combination, optionally ignoring modifier keys.
    /// </summary>
    /// <remarks>When ignoreModifiers is set to true, only the primary and secondary key codes are compared.
    /// If false, the comparison also includes the state of the Ctrl, Alt, and Shift modifiers.</remarks>
    /// <param name="keyCombination">The key combination to compare against the key event.</param>
    /// <param name="keyEvent">The key event to evaluate for a match with the key combination.</param>
    /// <param name="ignoreModifiers">true to ignore modifier keys (Ctrl, Alt, Shift) when comparing; otherwise, false.</param>
    /// <returns>true if the key event matches the key combination according to the specified criteria; otherwise, false.</returns>
    public static bool Equals(this KeyCombination keyCombination, KeyEvent keyEvent, bool ignoreModifiers = false)
    {
        if (ignoreModifiers)
        {
            return keyCombination.KeyCode == keyEvent.KeyCode && keyCombination.SecondKeyCode == keyEvent.KeyCode2;
        }
        return keyCombination.KeyCode == keyEvent.KeyCode
               && keyCombination.SecondKeyCode == keyEvent.KeyCode2
               && keyCombination.Ctrl == keyEvent.CtrlPressed
               && keyCombination.Alt == keyEvent.AltPressed
               && keyCombination.Shift == keyEvent.ShiftPressed;
    }

    /// <summary>
    ///     Determines whether the specified key combinations are equal, optionally ignoring modifier keys.
    /// </summary>
    /// <remarks>When ignoreModifiers is set to true, only the primary and secondary key codes are compared.
    /// If false, the comparison also includes the state of the Ctrl, Alt, and Shift modifiers.</remarks>
    /// <param name="keyCombination">The first key combination to compare.</param>
    /// <param name="other">The second key combination to compare with the first.</param>
    /// <param name="ignoreModifiers">true to ignore modifier keys (Ctrl, Alt, Shift) during comparison; otherwise, false.</param>
    /// <returns>true if the key combinations are considered equal based on the specified criteria; otherwise, false.</returns>
    public static bool Equals(this KeyCombination keyCombination, KeyCombination other, bool ignoreModifiers = false)
    {
        if (ignoreModifiers)
        {
            return keyCombination.KeyCode == other.KeyCode && keyCombination.SecondKeyCode == other.SecondKeyCode;
        }
        return keyCombination.KeyCode == other.KeyCode
               && keyCombination.SecondKeyCode == other.SecondKeyCode
               && keyCombination.Ctrl == other.Ctrl
               && keyCombination.Alt == other.Alt
               && keyCombination.Shift == other.Shift;
    }

    /// <summary>
    ///     Determines whether the specified key event matches the given key combination, optionally ignoring modifier keys.
    /// </summary>
    /// <remarks>When ignoreModifiers is set to true, only the primary and secondary key codes are compared.
    /// If false, the comparison also includes the state of the Ctrl, Alt, and Shift modifiers.</remarks>
    /// <param name="keyEvent">The key event to compare against the key combination.</param>
    /// <param name="other">The key combination to compare with the key event.</param>
    /// <param name="ignoreModifiers">true to ignore modifier keys (Ctrl, Alt, Shift) during comparison; otherwise, false.</param>
    /// <returns>true if the key event matches the key combination according to the specified criteria; otherwise, false.</returns>
    public static bool Equals(this KeyEvent keyEvent, KeyCombination other, bool ignoreModifiers = false)
    {
        if (ignoreModifiers)
        {
            return keyEvent.KeyCode == other.KeyCode && keyEvent.KeyCode2 == other.SecondKeyCode;
        }
        return keyEvent.KeyCode == other.KeyCode
               && keyEvent.KeyCode2 == other.SecondKeyCode
               && keyEvent.CtrlPressed == other.Ctrl
               && keyEvent.AltPressed == other.Alt
               && keyEvent.ShiftPressed == other.Shift;
    }

    /// <summary>
    ///     Determines whether the specified key event is equal to another key event, with an option to ignore modifier
    /// keys.
    /// </summary>
    /// <remarks>When ignoreModifiers is set to true, only the key codes are compared and the states of
    /// modifier keys are not considered. This can be useful when the comparison should focus solely on the key codes
    /// regardless of modifier key states.</remarks>
    /// <param name="keyEvent">The key event to compare.</param>
    /// <param name="other">The key event to compare with the current key event.</param>
    /// <param name="ignoreModifiers">true to ignore the state of modifier keys (Ctrl, Alt, Shift) during comparison; otherwise, false.</param>
    /// <returns>true if the key events are considered equal based on the specified criteria; otherwise, false.</returns>
    public static bool Equals(this KeyEvent keyEvent, KeyEvent other, bool ignoreModifiers = false)
    {
        if (ignoreModifiers)
        {
            return keyEvent.KeyCode == other.KeyCode && keyEvent.KeyCode2 == other.KeyCode2;
        }

        return keyEvent.KeyCode == other.KeyCode
               && keyEvent.KeyCode2 == other.KeyCode2
               && keyEvent.CtrlPressed == other.CtrlPressed
               && keyEvent.AltPressed == other.AltPressed
               && keyEvent.ShiftPressed == other.ShiftPressed;
    }
}