using Cairo;
using Gantry.Core.GameContent.GUI.RadialMenu.Abstractions;
using Gantry.Core.Maths;
using Vintagestory.API.MathTools;

namespace Gantry.Core.GameContent.GUI.RadialMenu;

/// <summary>
///     Represents a position for a radial menu element, handling its rendering, selection, and hover events.
/// </summary>
public class RadialElementPosition : IRadialElement, IDisposable
{
    private readonly static double _angleOffsetFromScreen = 4.71238899230957;
    private readonly ICoreClientAPI _api;
    private readonly IGuiAPI _guiApi;
    private LoadedTexture _backGroundTexture;
    private LoadedTexture _backGroundSelectedTexture;

    /// <summary>
    ///     Gets or sets the numerical position of the radial element.
    /// </summary>
    public int NumericalPosition { get; set; } = -1;

    /// <summary>
    ///     Gets or sets the middle X coordinate of the radial element.
    /// </summary>
    public int MiddleX { get; set; }

    /// <summary>
    ///     Gets or sets the middle Y coordinate of the radial element.
    /// </summary>
    public int MiddleY { get; set; }

    /// <summary>
    ///     Gets or sets the X offset of the radial element.
    /// </summary>
    public int XOffset { get; set; }

    /// <summary>
    ///     Gets or sets the Y offset of the radial element.
    /// </summary>
    public int YOffset { get; set; }

    /// <summary>
    ///     Gets or sets the angle of the radial element.
    /// </summary>
    public float Angle { get; set; }

    /// <summary>
    ///     Gets or sets the element-specific angle.
    /// </summary>
    public float ElementAngle { get; set; }

    /// <summary>
    ///     Gets or sets the gap between elements.
    /// </summary>
    public int Gape { get; set; } = 5;

    /// <summary>
    ///     Gets or sets the middle radius of the radial element.
    /// </summary>
    public int MidRadius { get; set; }

    /// <summary>
    ///     Gets or sets the thickness of the radial element.
    /// </summary>
    public int Thickness { get; set; }

    /// <summary>
    ///     Gets or sets the background texture of the radial element.
    /// </summary>
    public LoadedTexture BackGroundTexture { get => _backGroundTexture; set => _backGroundTexture = value; }

    /// <summary>
    ///     Gets or sets the background texture for the selected radial element.
    /// </summary>
    public LoadedTexture BackGroundSelectedTexture { get => _backGroundSelectedTexture; set => _backGroundSelectedTexture = value; }

    /// <summary>
    ///     Gets or sets the size of the background texture.
    /// </summary>
    public float BackGroundTextureSize { get; set; }

    /// <summary>
    ///     Gets or sets the icon of the radial element.
    /// </summary>
    public LoadedTexture Icon { get; set; }

    /// <summary>
    ///     Gets or sets the size of the icon.
    /// </summary>
    public IntXY IconSize { get; set; }

    /// <summary>
    ///     Gets or sets whether the icon should be automatically disposed when no longer needed.
    /// </summary>
    public bool AutoDisposeIcon { get; set; }

    /// <summary>
    ///     Gets or sets the scale of the icon.
    /// </summary>
    public float IconScale { get; set; } = 0.7f;

    /// <summary>
    ///     Gets or sets whether the element is being hovered.
    /// </summary>
    public bool Hover { get; set; }

    /// <summary>
    ///     Gets or sets the select event for the radial element.
    /// </summary>
    public Action SelectEvent { get; set; }

    /// <summary>
    ///     Gets or sets the hover event for the radial element.
    /// </summary>
    public Action<bool> HoverEvent { get; set; }

    /// <summary>
    ///     Gets the unique identifier of the radial element.
    /// </summary>
    public int Id { get; }

    /// <summary>
    ///     Initialises a new instance of the <see cref="RadialElementPosition"/> class.
    /// </summary>
    /// <param name="api">The core client API.</param>
    public RadialElementPosition(ICoreClientAPI api)
    {
        _api = api;
        _guiApi = api.Gui;
    }

    /// <summary>
    ///     Initialises a new instance of the <see cref="RadialElementPosition"/> class with an icon.
    /// </summary>
    /// <param name="api">The core client API.</param>
    /// <param name="icon">The icon to associate with the radial element.</param>
    public RadialElementPosition(ICoreClientAPI api, LoadedTexture icon)
        : this(api)
    {
        Icon = icon;
    }

    /// <summary>
    ///     Initialises a new instance of the <see cref="RadialElementPosition"/> class with an icon and selection event.
    /// </summary>
    /// <param name="api">The core client API.</param>
    /// <param name="icon">The icon to associate with the radial element.</param>
    /// <param name="onSelect">The action to execute when the element is selected.</param>
    public RadialElementPosition(ICoreClientAPI api, LoadedTexture icon, Action onSelect)
        : this(api, icon)
    {
        SelectEvent = onSelect;
    }

    /// <summary>
    ///     Updates the position and angles for the radial element.
    /// </summary>
    /// <param name="numericalPosition">The numerical position of the element.</param>
    /// <param name="xOffset">The X offset of the element.</param>
    /// <param name="yOffset">The Y offset of the element.</param>
    /// <param name="angle">The main angle of the element.</param>
    /// <param name="elementAngle">The element-specific angle.</param>
    public void UpdatePosition(
        int numericalPosition,
        int xOffset,
        int yOffset,
        float angle,
        float elementAngle)
    {
        XOffset = xOffset;
        YOffset = yOffset;
        Angle = angle;
        ElementAngle = elementAngle;
        NumericalPosition = numericalPosition;
    }

    /// <summary>
    ///     Sets the icon for the radial element, with an option for automatic disposal.
    /// </summary>
    /// <param name="texture">The texture for the icon.</param>
    /// <param name="autoDispose">Whether to automatically dispose the icon when no longer needed.</param>
    public void SetIcon(LoadedTexture texture, bool autoDispose)
    {
        if (Icon != null && AutoDisposeIcon && !Icon.Disposed)
            Icon.Dispose();
        Icon = texture;
        AutoDisposeIcon = true;
        UpdateIconSize(MaxIconSize());
    }

    /// <summary>
    ///     Begins the hover event for the radial element.
    /// </summary>
    public void OnHoverBegin()
    {
        Hover = true;
        HoverEvent?.Invoke(Hover);
    }

    /// <summary>
    ///     Ends the hover event for the radial element.
    /// </summary>
    public void OnHoverEnd()
    {
        Hover = false;
        HoverEvent?.Invoke(Hover);
    }

    /// <summary>
    ///     Executes the select event for the radial element.
    /// </summary>
    public void OnSelect()
    {
        SelectEvent?.Invoke();
    }

    /// <summary>
    ///     Updates the radius and thickness of the radial element.
    /// </summary>
    /// <param name="midRadius">The middle radius of the element.</param>
    /// <param name="thickness">The thickness of the element.</param>
    public void UpdateRadius(int midRadius, int thickness)
    {
        MidRadius = midRadius;
        Thickness = thickness;
    }

    /// <summary>
    ///     Redraws the radial element to a texture.
    /// </summary>
    public void ReDrawElementToTexture()
    {
        if (BackGroundTexture != null && !BackGroundTexture.Disposed)
        {
            BackGroundTexture.Dispose();
            BackGroundSelectedTexture.Dispose();
        }
        BackGroundTexture = new LoadedTexture(_api);
        BackGroundSelectedTexture = new LoadedTexture(_api);
        BackGroundTextureSize = (MidRadius + Thickness) * 2 + 10;
        var surface = new ImageSurface(0, (int)BackGroundTextureSize, (int)BackGroundTextureSize);
        var ctx = new Context(surface);
        CreateTexture(surface, ctx);
        ctx.Dispose();
        surface.Dispose();
        UpdateIconSize(MaxIconSize());
    }

    /// <summary>
    ///     Creates a texture for the background of the radial element.
    /// </summary>
    /// <param name="surface">The surface on which to draw the texture.</param>
    /// <param name="ctx">The drawing context used for rendering.</param>
    protected virtual void CreateTexture(ImageSurface surface, Context ctx)
    {
        PushPath(ctx);
        var dialogLightBgColor = GuiStyle.DialogLightBgColor;
        ctx.SetSourceRGBA(dialogLightBgColor[0], dialogLightBgColor[1], dialogLightBgColor[2], 0.4);
        ctx.LineWidth = 6.0;
        ctx.StrokePreserve();
        ctx.SetSourceRGBA(dialogLightBgColor[0], dialogLightBgColor[1], dialogLightBgColor[2], 0.7);
        ctx.Fill();
        _guiApi.LoadOrUpdateCairoTexture(surface, true, ref _backGroundTexture);
        ctx.Dispose();
        surface.Dispose();
        surface = new ImageSurface(0, (int)BackGroundTextureSize, (int)BackGroundTextureSize);
        ctx = new Context(surface);
        PushPath(ctx);
        ctx.SetSourceRGBA(dialogLightBgColor[0], dialogLightBgColor[1], dialogLightBgColor[2], 0.2);
        ctx.FillPreserve();
        ctx.SetSourceRGBA(dialogLightBgColor[0], dialogLightBgColor[1], dialogLightBgColor[2], 1.0);
        ctx.Stroke();
        _guiApi.LoadOrUpdateCairoTexture(surface, true, ref _backGroundSelectedTexture);
    }

    private void UpdateIconSize(int maxIconSize)
    {
        if (Icon is null || Icon.Disposed) return;
        var float2 = new FloatXY(Icon.Width, Icon.Height);
        if ((double)float2.Min <= 0.0)
        {
            IconSize = new IntXY(maxIconSize, maxIconSize);
        }
        else
        {
            var max = float2.Max;
            var num = maxIconSize / max * IconScale;
            IconSize = (IntXY)(float2 * num);
        }
    }

    private int MaxIconSize()
    {
        var thickness = Thickness;
        var num1 = ElementAngle / 2f;
        var num2 = AngleOffset(MidRadius);
        var num3 = Angle - num1 + num2;
        var int2 = new IntXY((int)(MidRadius * (double)GameMath.Sin(num3)), (int)(-MidRadius * (double)GameMath.Cos(num3)));
        var num4 = Angle + num1 - num2;
        var second = new IntXY((int)(MidRadius * (double)GameMath.Sin(num4)), (int)(-MidRadius * (double)GameMath.Cos(num4)));
        var num5 = (int)int2.Distance(second);
        return thickness >= num5 ? num5 : thickness;
    }

    private void PushPath(Context ctx)
    {
        var num1 = Thickness / 2f;
        var num2 = (double)BackGroundTextureSize / 2.0;
        var num3 = ElementAngle / 2f;
        var radius1 = MidRadius + num1;
        var num4 = AngleOffset(radius1);
        ctx.Arc(num2, num2, (double)radius1, (double)Angle - (double)num3 + _angleOffsetFromScreen + (double)num4, (double)Angle + (double)num3 + _angleOffsetFromScreen - (double)num4);
        var radius2 = MidRadius - num1;
        var num5 = AngleOffset(radius2);
        var num6 = (double)radius2 * (double)GameMath.Sin(Angle + num3 - num5) + num2;
        var num7 = -(double)radius2 * (double)GameMath.Cos(Angle + num3 - num5) + num2;
        ctx.LineTo(num6, num7);
        ctx.ArcNegative(num2, num2, (double)radius2, (double)Angle + (double)num3 + _angleOffsetFromScreen - (double)num5, (double)Angle - (double)num3 + _angleOffsetFromScreen + (double)num5);
        ctx.ClosePath();
    }

    /// <summary>
    ///     Disposes of the resources used by this radial element.
    /// </summary>
    public void Dispose()
    {
        BackGroundTexture?.Dispose();
        BackGroundSelectedTexture?.Dispose();
        Icon?.Dispose();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///     Renders the menu element based on its current state.
    /// </summary>
    public void RenderMenuElement()
    {
        var rapi = ApiEx.Client.Render;
        if (BackGroundTexture == null)
            return;
        var num = (int)((double)BackGroundTextureSize / 2.0);
        if (Hover)
            rapi.Render2DLoadedTexture(BackGroundSelectedTexture, MiddleX - num, MiddleY - num, 50f);
        else
            rapi.Render2DLoadedTexture(BackGroundTexture, MiddleX - num, MiddleY - num, 50f);
        if (Icon != null && !Icon.Disposed)
        {
            var int2 = IconSize / 2;
            rapi.Render2DTexture(Icon.TextureId, MiddleX + XOffset - int2.X, MiddleY + YOffset - int2.Y, IconSize.X, IconSize.Y, 50f, null);
        }
    }

    /// <summary>
    ///     Calculates the angle offset based on the given radius.
    /// </summary>
    /// <param name="radius">The radius to calculate the offset for.</param>
    /// <returns>The angle offset.</returns>
    protected float AngleOffset(float radius) => Gape / radius;

    /// <summary>
    ///     Updates the middle position of the element.
    /// </summary>
    /// <param name="x">The new x-coordinate of the middle position.</param>
    /// <param name="y">The new y-coordinate of the middle position.</param>
    public void UpdateMiddlePosition(int x, int y)
    {
        MiddleX = x;
        MiddleY = y;
    }

    /// <summary>
    ///     Gets the numeric ID of the element.
    /// </summary>
    /// <value>The numeric ID of the element.</value>
    public int NumericID => NumericalPosition;

    /// <summary>
    ///     Gets the offset values for the element.
    /// </summary>
    /// <returns>The offset as a float2.</returns>
    public FloatXY GetOffset() => new(XOffset, YOffset);
}