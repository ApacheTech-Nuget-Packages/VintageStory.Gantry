using Gantry.Core.Annotation;
using Gantry.Extensions;
using Gantry.Extensions.Api;

namespace Gantry.Utilities;

/// <summary>
///     Manages automatic hotkey activation with double-tap toggle detection and interrupt key support.
/// </summary>
[ClientSide]
public class AutoHotkey : IDisposable
{
    private readonly ICoreGantryAPI _gantry;
    private readonly KeyEvent _keyEvent;
    private readonly AutoResetValue<int> _trigger;
    private readonly string[] _interrupts;

    /// <summary>
    ///     The unique code identifying this hotkey in the input system.
    /// </summary>
    public string HotkeyCode { get; private set; }

    /// <summary>
    ///     Whether this hotkey is enabled and can be activated.
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    ///     Whether this hotkey is currently in an active (pressed) state.
    /// </summary>
    public bool Active { get; private set; }

    /// <summary>
    ///     Initialises a new instance of the <see cref="AutoHotkey"/> class.
    /// </summary>
    /// <param name="gantry">The Gantry API instance.</param>
    /// <param name="hotkeyCode">The unique code identifying the hotkey.</param>
    /// <param name="enabled">Whether the hotkey should be enabled initially.</param>
    /// <param name="interrupts">Optional array of key codes that will deactivate the hotkey when pressed.</param>
    public AutoHotkey(ICoreGantryAPI gantry, string hotkeyCode, bool enabled = true, string[]? interrupts = null)
    {
        _gantry = gantry;
        _interrupts = interrupts ?? [];
        HotkeyCode = hotkeyCode;
        Enabled = enabled;

        _keyEvent = CurrentMapping.ToKeyEvent();
        _trigger = new(TimeSpan.FromMilliseconds(200));
    }

    /// <summary>
    ///     The current key combination mapped to this hotkey.
    /// </summary>
    public KeyCombination CurrentMapping
        => _gantry.Capi.Input.GetHotKeyByCode(HotkeyCode).CurrentMapping;

    /// <summary>
    ///     Sets the enabled state of the hotkey. If disabled, the hotkey will be deactivated if it is currently active.
    /// </summary>
    /// <param name="enabled">The new enabled state for the hotkey.</param>
    public void SetEnabled(bool enabled)
    {
        Enabled = enabled;
        if (!enabled) Deactivate();
    }

    /// <summary>
    ///     Toggles between active and inactive states.
    /// </summary>
    public void Toggle() => Active.ActIf(Deactivate, Activate);

    /// <summary>
    ///     Activates the hotkey, simulating a key down event.
    /// </summary>
    public void Activate()
    {
        if (!Enabled)
        {
            Deactivate();
            return;
        }
        if (Active) return;
        Active = true;
        _gantry.Capi.ClientMain.OnKeyDown(_keyEvent);
    }

    /// <summary>
    ///     Deactivates the hotkey, simulating a key up event.
    /// </summary>
    public void Deactivate()
    {
        if (!Active) return;
        Active = false;
        _gantry.Capi.ClientMain.OnKeyUp(_keyEvent);
    }

    /// <summary>
    ///     Processes a key event for double-tap toggle detection.
    ///     If the same key is pressed twice within 200ms, the hotkey state is toggled.
    /// </summary>
    /// <param name="e">The key event to process.</param>
    public void Trigger(KeyEvent e)
    {
        if (e.KeyCode != CurrentMapping.KeyCode) return;
        if (e.KeyCode == _trigger.Value) Toggle();
        else _trigger.Set(e.KeyCode);
    }

    /// <summary>
    ///     Processes a key event for interrupt detection, deactivating the hotkey if an interrupt key is pressed.
    /// </summary>
    /// <param name="e">The key event to process.</param>
    public void Interrupt(KeyEvent e)
    {
        if (!_interrupts.Contains(e.KeyCode.ToString())) return;
        Deactivate();
    }

    /// <summary>
    ///     Handles a key up event, deactivating the hotkey if it matches the current mapping.
    /// </summary>
    /// <param name="e">The key event to handle.</param>
    /// <returns><c>true</c> if the event was handled; otherwise, <c>false</c>.</returns>
    public bool OnKeyUp(KeyEvent e)
    {
        if (e.KeyCode != _keyEvent.KeyCode || e.KeyCode2 != _keyEvent.KeyCode2) return e.Handled = false;
        Deactivate();
        return e.Handled = true;
    }

    /// <summary>
    ///     Handles a key down event, activating the hotkey if it matches the current mapping.
    /// </summary>
    /// <param name="e">The key event to handle.</param>
    /// <returns><c>true</c> if the event was handled; otherwise, <c>false</c>.</returns>
    public bool OnKeyDown(KeyEvent e)
    {
        if (e.KeyCode != _keyEvent.KeyCode || e.KeyCode2 != _keyEvent.KeyCode2) return e.Handled = false;
        Activate();
        return e.Handled = true;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _trigger.Dispose();
    }
}