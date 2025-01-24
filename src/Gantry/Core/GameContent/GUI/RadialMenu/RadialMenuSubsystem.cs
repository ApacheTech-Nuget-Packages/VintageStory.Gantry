using Microsoft.Win32;

namespace Gantry.Core.GameContent.GUI.RadialMenu;

/// <summary>
///     Manages the rendering of radial menus and their elements.
/// </summary>
public class RadialMenuSubsystem : ClientSubsystem, IRenderer
{
    private bool Keyboard;
    private int CurrnetKeyBind;

    private RadialMenu CurrentlyOpened;
    private Dictionary<int, RadialItemMenu> KeybordBinding = [];
    private List<RadialItemMenu> MouseBinding = [];

    private bool Clicked;
    private long HoldThreshold;

    private DateTime time;
    private bool waitForRelease = false;
    private bool waitForBegin = false;

    /// <summary>
    ///     Gets or sets the scale factor for the radial menu.
    /// </summary>
    /// <remarks>
    ///     A value of 1 represents the default size. Values greater than 1 will scale the menu up,
    ///     and values less than 1 will scale it down.
    /// </remarks>
    public float Scale { get; set; } = 1f;

    /// <summary>
    ///     Gets or sets the duration, in milliseconds, for which a button must be held to activate.
    /// </summary>
    /// <remarks>
    ///     A default value of 250ms is typically used for a responsive user experience.
    /// </remarks>
    public int ButtonHoldDuration { get; set; } = 250;

    private List<object> _registry = [];

    /// <inheritdoc />
    public override void StartClientSide(ICoreClientAPI capi)
    {
        if (!_registry.Any()) return;
        capi.Event.LevelFinalize += OnLevelFinalise;
        
    }

    private void OnLevelFinalise()
    {

    }

    #region Keyboard Event Handlers

    #endregion

    #region Mouse Event Handlers

    #endregion

    #region Menu Registration

    #endregion

    #region Implementation of IRenderer

    /// <inheritdoc />
    public void OnRenderFrame(float deltaTime, EnumRenderStage stage)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public double RenderOrder { get; }

    /// <inheritdoc />
    public int RenderRange { get; }

    #endregion
}