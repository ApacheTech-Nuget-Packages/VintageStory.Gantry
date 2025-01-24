namespace Gantry.Core.GameContent.GUI.RadialMenu;

/// <summary>
///     Represents a radial item menu with optional keyboard or mouse bindings.
/// </summary>
public class RadialItemMenu
{
    /// <summary>
    ///     Initialises a new instance of the <see cref="RadialItemMenu"/> class with a keyboard binding.
    /// </summary>
    /// <param name="id">The identifier for the radial item menu.</param>
    /// <param name="menu">The parent radial menu instance.</param>
    /// <param name="key">The keyboard key associated with this menu item.</param>
    public RadialItemMenu(string id, RadialMenu menu, GlKeys key)
        : this(id, menu, false, (int)key)
    {
    }

    /// <summary>
    ///     Initialises a new instance of the <see cref="RadialItemMenu"/> class with a mouse binding.
    /// </summary>
    /// <param name="id">The identifier for the radial item menu.</param>
    /// <param name="menu">The parent radial menu instance.</param>
    /// <param name="key">The mouse button associated with this menu item.</param>
    public RadialItemMenu(string id, RadialMenu menu, EnumMouseButton key)
        : this(id, menu, true, (int)key)
    {
    }

    /// <summary>
    ///     Initialises a new instance of the <see cref="RadialItemMenu"/> class with custom binding settings.
    /// </summary>
    /// <param name="id">The identifier for the radial item menu.</param>
    /// <param name="menu">The parent radial menu instance.</param>
    /// <param name="mouseBinding">
    ///     A value indicating whether the binding is a mouse binding (true) or a keyboard binding (false).
    /// </param>
    /// <param name="bindId">The identifier of the binding (key or mouse button).</param>
    public RadialItemMenu(string id, RadialMenu menu, bool mouseBinding, int bindId)
    {
        Id = id;
        Menu = menu;
        MouseBinding = mouseBinding;
        BindId = bindId;
    }

    /// <summary>
    ///     Gets a value indicating whether the binding is a mouse binding.
    /// </summary>
    public bool MouseBinding { get; }

    /// <summary>
    ///     Gets the identifier of the binding (key or mouse button).
    /// </summary>
    public int BindId { get; }

    /// <summary>
    ///     Gets the parent radial menu instance.
    /// </summary>
    public RadialMenu Menu { get; }

    /// <summary>
    ///     Gets the identifier for the radial item menu.
    /// </summary>
    public string Id { get; }

    /// <summary>
    ///     A predicate that can be used to determine if an action should occur when the menu is opened.
    /// </summary>
    public Predicate<RadialItemMenu> RiseOnOpen { get; set; }
}