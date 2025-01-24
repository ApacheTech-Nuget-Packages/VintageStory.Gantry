namespace Gantry.Core.GameContent.GUI.Abstractions;

/// <summary>
///     Represents a composable GUI window that can be used as a part of a composite GUI screen.
/// </summary>
public interface IGuiWindow
{
    /// <summary>
    ///     The name to give to this window, within the parent's composer cache.
    /// </summary>
    string Key { get; }

    /// <summary>
    ///     Composes the GUI window.
    /// </summary>
    /// <param name="parent">The parent <see cref="GuiDialog"/> that hosts this window.</param>
    /// <returns></returns>
    GuiComposer Compose(GuiDialog parent);

    /// <summary>
    ///     Refreshes the values within the GUI window.
    /// </summary>
    void RefreshValues();
}