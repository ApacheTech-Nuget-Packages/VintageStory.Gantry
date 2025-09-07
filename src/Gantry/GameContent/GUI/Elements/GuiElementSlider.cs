using Cairo;
using System.Numerics;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;

namespace Gantry.GameContent.GUI.Elements;

/// <summary>
///     Provides a slider GUI element for use in mod UIs, allowing users to select a value within a specified range, 
///     with optional step increments, alarm value highlighting, and custom tooltips, or resting text.
/// </summary>
/// <param name="capi">The client API instance.</param>
/// <param name="onNewSliderValue">A callback invoked when the slider value changes.</param>
/// <param name="bounds">The element bounds for the slider.</param>
public class GuiElementSlider<T>(ICoreClientAPI capi, ActionConsumable<T> onNewSliderValue, ElementBounds bounds)
    : GuiElementControl(capi, bounds) where T : struct, INumber<T>
{
    /// <summary>
    ///     The unscaled height of the slider element.
    /// </summary>
    public const double UnscaledHeight = 20;

    /// <summary>
    ///     The unscaled padding for the slider element.
    /// </summary>
    public const double UnscaledPadding = 4;

    private readonly double _unscaledHandleWidth = 15;
    private readonly double _unscaledHandleHeight = 35;

    private T _minValue;
    private T _maxValue = T.CreateChecked(100);
    private T _step = T.One;
    private int _decimalPlaces;
    private T _currentValue;
    private T _alarmValue;
    private string _unit = "";

    private HashSet<T> _skipValues = [];
    private readonly List<T> _allowValues = [];

    private bool _mouseDownOnSlider;
    private bool _mouseOnSlider;
    private bool _triggerOnMouseUp;
    private bool _didChangeValue;

    private double _handleWidth;
    private double _handleHeight;
    private double _padding;

    private readonly LoadedTexture _alarmValueTexture = new(capi);
    private LoadedTexture _handleTexture = new(capi);
    private LoadedTexture _hoverTextTexture = new(capi);
    private LoadedTexture _restingTextTexture = new(capi);
    private LoadedTexture _sliderFillTexture = new(capi);
    private GuiElementStaticText? _textElem;
    private GuiElementStaticText? _textElemResting;
    private Rectangled? _alarmTextureRect;

    private readonly ActionConsumable<T> _onNewSliderValue = onNewSliderValue;

    /// <summary>
    ///     A function to provide a tooltip for the slider, based on the current value.
    /// </summary>
    public System.Func<T, string>? OnSliderTooltip { get; private set; }

    /// <summary>
    ///     A function to provide resting text for the slider, based on the current value.
    /// </summary>
    public System.Func<T, string>? OnSliderRestingText { get; private set; }

    /// <summary>
    ///     Whether the tooltip may exceed the clip bounds of the element.
    /// </summary>
    public bool TooltipExceedClipBounds { get; set; }

    /// <summary>
    ///     Whether to show text when the slider is at rest.
    /// </summary>
    public bool ShowTextWhenResting { get; set; }

    /// <summary>
    ///     Whether the slider is enabled.
    ///     When set, updates the visual state and alarm value texture as needed.
    /// </summary>
    public override bool Enabled
    {
        get => base.Enabled;
        set
        {
            enabled = value;
            ComposeHandleElement();
            if (_alarmValue > _minValue && _alarmValue < _maxValue)
            {
                MakeAlarmValueTexture();
            }
            ComposeHoverTextElement();
            ComposeRestingTextElement();
            ComposeFillTexture();
        }
    }

    /// <summary>
    ///     Gets whether the slider is focusable.
    /// </summary>
    public override bool Focusable => enabled;

    /// <summary>
    ///     Composes the static and dynamic elements of the slider, including handle, fill, and text overlays.
    /// </summary>
    /// <param name="ctxStatic">The Cairo context for static drawing.</param>
    /// <param name="surfaceStatic">The Cairo image surface for static drawing.</param>
    public override void ComposeElements(Context ctxStatic, ImageSurface surfaceStatic)
    {
        _handleWidth = scaled(_unscaledHandleWidth) * Scale;
        _handleHeight = scaled(_unscaledHandleHeight) * Scale;
        _padding = scaled(4.0) * Scale;
        Bounds.CalcWorldBounds();
        ctxStatic.SetSourceRGBA(0.0, 0.0, 0.0, 0.2);
        RoundRectangle(ctxStatic, Bounds.drawX, Bounds.drawY, Bounds.InnerWidth, Bounds.InnerHeight, 1.0);
        ctxStatic.Fill();
        EmbossRoundRectangleElement(ctxStatic, Bounds, inverse: true, 1, 1);
        _ = Bounds.InnerWidth;
        _ = _padding;
        _ = Bounds.InnerHeight;
        _ = _padding;
        ComposeHandleElement();
        ComposeFillTexture();
        ComposeHoverTextElement();
        ComposeRestingTextElement();
    }

    /// <summary>
    ///     Composes the handle element for the slider, including its visual style and texture.
    /// </summary>
    public void ComposeHandleElement()
    {
        var imageSurface = new ImageSurface(Format.Argb32, (int)_handleWidth + 4, (int)_handleHeight + 4);
        var context = genContext(imageSurface);
        context.SetSourceRGBA(1.0, 1.0, 1.0, 0.0);
        context.Paint();
        RoundRectangle(context, 2.0, 2.0, _handleWidth, _handleHeight, 1.0);
        if (!enabled)
        {
            context.SetSourceRGB(43.0 / 255.0, 11.0 / 85.0, 8.0 / 85.0);
            context.FillPreserve();
        }
        fillWithPattern(api, context, woodTextureName, nearestScalingFiler: false, preserve: true, enabled ? 255 : 159, 0.5f);
        context.SetSourceRGB(43.0 / 255.0, 11.0 / 85.0, 8.0 / 85.0);
        context.LineWidth = 2.0;
        context.Stroke();
        generateTexture(imageSurface, ref _handleTexture);
        context.Dispose();
        imageSurface.Dispose();
    }

    /// <summary>
    ///     Composes the hover text element, which appears when the slider is hovered or dragged.
    /// </summary>
    public void ComposeHoverTextElement()
    {
        var elementBounds = new ElementBounds().WithFixedPadding(7.0).WithParent(ElementBounds.Empty);
        var text = ((IFormattable)_currentValue).ToString($"F{_decimalPlaces}", null) + _unit;
        if (OnSliderTooltip is not null)
        {
            text = OnSliderTooltip(_currentValue);
        }
        _textElem = new GuiElementStaticText(api, text, EnumTextOrientation.Center, elementBounds, CairoFont.WhiteMediumText().WithFontSize((float)GuiStyle.SubNormalFontSize));
        _textElem.Font.UnscaledFontsize = GuiStyle.SmallishFontSize;
        _textElem.AutoBoxSize();
        _textElem.Bounds.CalcWorldBounds();
        var imageSurface = new ImageSurface(Format.Argb32, (int)elementBounds.OuterWidth, (int)elementBounds.OuterHeight);
        var context = genContext(imageSurface);
        context.SetSourceRGBA(1.0, 1.0, 1.0, 0.0);
        context.Paint();
        context.SetSourceRGBA(GuiStyle.DialogStrongBgColor);
        RoundRectangle(context, 0.0, 0.0, elementBounds.OuterWidth, elementBounds.OuterHeight, GuiStyle.ElementBGRadius);
        context.FillPreserve();
        var dialogStrongBgColor = GuiStyle.DialogStrongBgColor;
        context.SetSourceRGBA(dialogStrongBgColor[0] / 2.0, dialogStrongBgColor[1] / 2.0, dialogStrongBgColor[2] / 2.0, dialogStrongBgColor[3]);
        context.Stroke();
        _textElem.ComposeElements(context, imageSurface);
        generateTexture(imageSurface, ref _hoverTextTexture);
        context.Dispose();
        imageSurface.Dispose();
    }

    /// <summary>
    ///     Composes the resting text element, which appears when the slider is at rest and ShowTextWhenResting is true.
    /// </summary>
    public void ComposeRestingTextElement()
    {
        var elementBounds = new ElementBounds().WithFixedPadding(7.0).WithParent(ElementBounds.Empty);
        var currentValueDouble = double.CreateChecked(_currentValue);
        var minValueDouble = double.CreateChecked(_minValue);
        var maxValueDouble = double.CreateChecked(_maxValue);
        var sliderRange = Bounds.InnerWidth - 2.0 * _padding - _handleWidth / 2.0;
        var fillWidth = sliderRange * (currentValueDouble - minValueDouble) / (maxValueDouble - minValueDouble);
        var text = ((IFormattable)_currentValue).ToString($"F{_decimalPlaces}", null) + _unit;
        if (OnSliderRestingText is not null)
        {
            text = OnSliderRestingText(_currentValue);
        }
        else if (OnSliderTooltip is not null)
        {
            text = OnSliderTooltip(_currentValue);
        }
        _textElemResting = new GuiElementStaticText(api, text, EnumTextOrientation.Center, elementBounds, CairoFont.WhiteSmallText());
        _textElemResting.AutoBoxSize();
        _textElemResting.Bounds.CalcWorldBounds();
        _textElemResting.Bounds.fixedY = ((int)(scaled(30.0) * Scale) - _textElemResting.Font.GetFontExtents().Height) / 2.0 / RuntimeEnv.GUIScale;
        if (!enabled)
        {
            _textElemResting.Font.Color[3] = 0.35;
        }
        if (fillWidth - 10.0 >= _textElemResting.Bounds.InnerWidth)
        {
            _textElemResting.Font.Color =
            [
                0.0,
                0.0,
                0.0,
                enabled ? 1.0 : 0.5
            ];
        }
        var imageSurface = new ImageSurface(Format.Argb32, (int)elementBounds.OuterWidth, (int)elementBounds.OuterHeight);
        var context = genContext(imageSurface);
        _textElemResting.ComposeElements(context, imageSurface);
        generateTexture(imageSurface, ref _restingTextTexture);
        context.Dispose();
        imageSurface.Dispose();
    }

    /// <summary>
    ///     Composes the fill texture for the slider, representing the filled portion of the slider bar.
    /// </summary>
    public void ComposeFillTexture()
    {
        var currentValueDouble = double.CreateChecked(_currentValue);
        var minValueDouble = double.CreateChecked(_minValue);
        var maxValueDouble = double.CreateChecked(_maxValue);
        var sliderRange = Bounds.InnerWidth - 2.0 * _padding - _handleWidth / 2.0;
        var fillWidth = sliderRange * (currentValueDouble - minValueDouble) / (maxValueDouble - minValueDouble);
        var fillHeight = Bounds.InnerHeight - 2.0 * _padding;
        var imageSurface = new ImageSurface(Format.Argb32, (int)(fillWidth + 5.0), (int)fillHeight);
        var context = genContext(imageSurface);
        var pattern = getPattern(api, waterTextureName, doCache: true, enabled ? 255 : 127, 0.5f);
        RoundRectangle(context, 0.0, 0.0, imageSurface.Width, imageSurface.Height, 1.0);
        if (enabled)
        {
            context.SetSourceRGBA(0.0, 0.0, 0.0, 1.0);
        }
        else
        {
            context.SetSourceRGBA(0.15, 0.15, 0.0, 0.65);
        }
        context.FillPreserve();
        context.SetSource(pattern);
        context.Fill();
        generateTexture(imageSurface, ref _sliderFillTexture);
        context.Dispose();
        imageSurface.Dispose();
    }

    /// <summary>
    ///     Renders the interactive elements of the slider, including the handle, fill, alarm, and text overlays.
    /// </summary>
    /// <param name="deltaTime">The time elapsed since the last frame.</param>
    public override void RenderInteractiveElements(float deltaTime)
    {
        var alarmValueDouble = double.CreateChecked(_alarmValue);
        var minValueDouble = double.CreateChecked(_minValue);
        var maxValueDouble = double.CreateChecked(_maxValue);
        var alarmPercent = (alarmValueDouble - minValueDouble) / (maxValueDouble - minValueDouble);
        if (alarmPercent > 0f && _alarmValueTexture.TextureId > 0)
        {
            api.Render.RenderTexture(_alarmValueTexture.TextureId, Bounds.renderX + _alarmTextureRect!.X, Bounds.renderY + _alarmTextureRect.Y, _alarmTextureRect.Width, _alarmTextureRect.Height);
        }
        var currentValueDouble = double.CreateChecked(_currentValue);
        var sliderRange = Bounds.InnerWidth - 2.0 * _padding - _handleWidth / 2.0;
        var fillWidth = sliderRange * (currentValueDouble - minValueDouble) / (maxValueDouble - minValueDouble);
        var fillHeight = Bounds.InnerHeight - 2.0 * _padding;
        var handleOffsetY = (_handleHeight - Bounds.OuterHeight + _padding) / 2.0;
        api.Render.Render2DTexturePremultipliedAlpha(_sliderFillTexture.TextureId, Bounds.renderX + _padding, Bounds.renderY + _padding, (int)(fillWidth + 5.0), (int)fillHeight);
        api.Render.Render2DTexturePremultipliedAlpha(_handleTexture.TextureId, Bounds.renderX + fillWidth, Bounds.renderY - handleOffsetY, (int)_handleWidth + 4, (int)_handleHeight + 4);
        if (_mouseDownOnSlider || _mouseOnSlider)
        {
            if (TooltipExceedClipBounds) api.Render.PopScissor();
            var bounds = _textElem!.Bounds;
            api.Render.Render2DTexturePremultipliedAlpha(_hoverTextTexture.TextureId, (int)(Bounds.renderX + _padding + fillWidth - bounds.OuterWidth / 2.0 + _handleWidth / 2.0), (int)(Bounds.renderY - scaled(20.0) - bounds.OuterHeight), bounds.OuterWidthInt, bounds.OuterHeightInt, 300f);
            if (TooltipExceedClipBounds) api.Render.PushScissor(InsideClipBounds);
        }
        if (ShowTextWhenResting)
        {
            api.Render.PushScissor(Bounds, stacking: true);
            var bounds2 = _textElemResting!.Bounds;
            var posX = fillWidth - 10.0 < bounds2.InnerWidth ? Bounds.renderX + _padding + fillWidth - bounds2.OuterWidth / 2.0 + _handleWidth / 2.0 + _restingTextTexture.Width / 2 + 10.0 : (int)Bounds.renderX;
            api.Render.Render2DTexturePremultipliedAlpha(_restingTextTexture.TextureId, posX, Bounds.renderY + (fillHeight - bounds2.OuterHeight - _padding / 2.0) / 2.0, bounds2.OuterWidthInt, bounds2.OuterHeightInt, 300f);
            api.Render.PopScissor();
        }
    }

    /// <summary>
    ///     Creates or updates the alarm value texture, which highlights a region of the slider when the alarm value is set.
    /// </summary>
    private void MakeAlarmValueTexture()
    {
        var alarmValueDouble = double.CreateChecked(_alarmValue);
        var minValueDouble = double.CreateChecked(_minValue);
        var maxValueDouble = double.CreateChecked(_maxValue);
        var alarmPercent = (alarmValueDouble - minValueDouble) / (maxValueDouble - minValueDouble);
        var sliderRange = Bounds.InnerWidth - 2.0 * _padding;
        _alarmTextureRect = new Rectangled
        {
            X = _padding + sliderRange * alarmPercent,
            Y = _padding,
            Width = sliderRange * (1f - alarmPercent),
            Height = Bounds.InnerHeight - 2.0 * _padding
        };
        var imageSurface = new ImageSurface(Format.Argb32, (int)_alarmTextureRect.Width, (int)_alarmTextureRect.Height);
        var context = genContext(imageSurface);
        context.SetSourceRGBA(1.0, 0.0, 1.0, enabled ? 0.4 : 0.25);
        RoundRectangle(context, 0.0, 0.0, _alarmTextureRect.Width, _alarmTextureRect.Height, GuiStyle.ElementBGRadius);
        context.Fill();
        generateTexture(imageSurface, ref _alarmValueTexture.TextureId);
        context.Dispose();
        imageSurface.Dispose();
    }

    /// <summary>
    ///     Handles mouse down events on the slider, updating the value if the slider is enabled and the mouse is within bounds.
    /// </summary>
    /// <param name="api">The client API instance.</param>
    /// <param name="args">The mouse event arguments.</param>
    public override void OnMouseDownOnElement(ICoreClientAPI api, MouseEvent args)
    {
        if (!enabled || !Bounds.PointInside(api.Input.MouseX, api.Input.MouseY)) return;
        args.Handled = UpdateValue(api.Input.MouseX);
        _mouseDownOnSlider = true;
    }

    /// <summary>
    ///     Handles mouse up events on the slider, triggering the value change callback if needed.
    /// </summary>
    /// <param name="api">The client API instance.</param>
    /// <param name="args">The mouse event arguments.</param>
    public override void OnMouseUp(ICoreClientAPI api, MouseEvent args)
    {
        _mouseDownOnSlider = false;
        if (!enabled) return;
        if (_onNewSliderValue is not null && _didChangeValue && _triggerOnMouseUp)
        {
            _onNewSliderValue(_currentValue);
        }
        _didChangeValue = false;
    }

    /// <summary>
    ///     Handles mouse move events on the slider, updating the value if the slider is enabled and being dragged.
    /// </summary>
    /// <param name="api">The client API instance.</param>
    /// <param name="args">The mouse event arguments.</param>
    public override void OnMouseMove(ICoreClientAPI api, MouseEvent args)
    {
        _mouseOnSlider = Bounds.PointInside(api.Input.MouseX, api.Input.MouseY);
        if (!enabled || !_mouseDownOnSlider) return;
        args.Handled = UpdateValue(api.Input.MouseX);
    }

    /// <summary>
    ///     Handles mouse wheel events on the slider, incrementing or decrementing the value as appropriate.
    /// </summary>
    /// <param name="api">The client API instance.</param>
    /// <param name="args">The mouse wheel event arguments.</param>
    public override void OnMouseWheel(ICoreClientAPI api, MouseWheelEventArgs args)
    {
        if (enabled && Bounds.PointInside(api.Input.MouseX, api.Input.MouseY))
        {
            args.SetHandled();
            var num = Math.Sign(args.deltaPrecise);
            if ((_currentValue > _minValue || num >= 0) && (_currentValue < _maxValue || num <= 0))
            {
                _currentValue = _allowValues[_allowValues.IndexOf(_currentValue) + num];
                ComposeHoverTextElement();
                ComposeRestingTextElement();
                ComposeFillTexture();
                _onNewSliderValue?.Invoke(_currentValue);
            }
        }
    }

    /// <summary>
    ///     Handles key down events for keyboard-based slider adjustment.
    /// </summary>
    /// <param name="api">The client API instance.</param>
    /// <param name="args">The key event arguments.</param>
    public override void OnKeyDown(ICoreClientAPI api, KeyEvent args)
    {
        if (!HasFocus)
        {
            return;
        }
        var num = 0;
        if (args.KeyCode == 47)
        {
            if (_currentValue <= _allowValues.First())
            {
                return;
            }
            num = -1;
        }
        else if (args.KeyCode == 48)
        {
            if (_currentValue >= _allowValues.Last())
            {
                return;
            }
            num = 1;
        }
        if (num != 0)
        {
            _currentValue = _allowValues[_allowValues.IndexOf(_currentValue) + num];
            ComposeHoverTextElement();
            ComposeRestingTextElement();
            ComposeFillTexture();
            _onNewSliderValue?.Invoke(_currentValue);
        }
    }

    /// <summary>
    ///     Sets whether the value change callback should only be triggered on mouse up.
    /// </summary>
    /// <param name="trigger">True to trigger only on mouse up; otherwise, false.</param>
    public void TriggerOnlyOnMouseUp(bool trigger = true) 
        => _triggerOnMouseUp = trigger;

    /// <summary>
    ///     Updates the slider value based on the given mouse X position.
    /// </summary>
    /// <param name="mouseX">The X position of the mouse.</param>
    /// <returns>True if the value was updated; otherwise, false.</returns>
    private bool UpdateValue(double mouseX)
    {
        var minValueDouble = double.CreateChecked(_minValue);
        var maxValueDouble = double.CreateChecked(_maxValue);
        var currentValueDouble = double.CreateChecked(_currentValue);
        var sliderRange = Bounds.InnerWidth - 2.0 * _padding - _handleWidth / 2.0;
        var clampedMouseX = GameMath.Clamp(mouseX - Bounds.renderX - _padding, 0.0, sliderRange);
        var interpolatedValue = minValueDouble + (maxValueDouble - minValueDouble) * clampedMouseX / sliderRange;
        var closestAllowed = _allowValues.Count == 0 ? _currentValue : _allowValues.OrderBy(item => Math.Abs(interpolatedValue - double.CreateChecked(item))).First();
        var rounded = RoundValue(closestAllowed);
        if (rounded.Equals(_currentValue)) return true;
        _didChangeValue = true;
        _currentValue = rounded;
        ComposeHoverTextElement();
        ComposeRestingTextElement();
        ComposeFillTexture();
        return _onNewSliderValue is null || _triggerOnMouseUp || _onNewSliderValue(_currentValue);
    }

    /// <summary>
    ///     Sets the alarm value for the slider, which highlights a region of the slider bar.
    /// </summary>
    /// <param name="value">The alarm value to set.</param>
    public void SetAlarmValue(T value)
    {
        _alarmValue = value;
        MakeAlarmValueTexture();
    }

    /// <summary>
    ///     Fills the list of allowed values for the slider, based on the min, max, step, and skip values.
    /// </summary>
    private void FillAllowedValues()
    {
        _allowValues.Clear();
        var min = double.CreateChecked(_minValue);
        var max = double.CreateChecked(_maxValue);
        var step = double.CreateChecked(_step);
        var epsilon = 1e-8;
        var range = max - min;
        var steps = (int)Math.Round(range / step);
        var expectedMax = min + steps * step;
        if (Math.Abs(expectedMax - max) > epsilon)
        {
            // Clamp max to the nearest valid value
            max = min + steps * step;
            _maxValue = T.CreateChecked(max);
        }
        for (int i = 0; i <= steps; i++)
        {
            var value = min + i * step;
            var tValue = RoundValue(T.CreateChecked(value));
            if (_skipValues.Contains(tValue)) continue;
            _allowValues.Add(tValue);
        }
    }

    /// <summary>
    ///     Sets the skip values for the slider, which are values that cannot be selected.
    /// </summary>
    /// <param name="skipValues">A set of values to skip.</param>
    public void SetSkipValues(HashSet<T> skipValues)
    {
        _skipValues = skipValues;
        FillAllowedValues();
    }

    /// <summary>
    ///     Clears all skip values from the slider.
    /// </summary>
    public void ClearSkipValues()
    {
        _skipValues.Clear();
        FillAllowedValues();
    }

    /// <summary>
    ///     Adds a single skip value to the slider.
    /// </summary>
    /// <param name="skipValue">The value to skip.</param>
    public void AddSkipValue(T skipValue)
    {
        _skipValues.Add(skipValue);
        _allowValues.Remove(skipValue);
    }

    /// <summary>
    ///     Removes a single skip value from the slider.
    /// </summary>
    /// <param name="skipValue">The value to remove from the skip list.</param>
    public void RemoveSkipValue(T skipValue)
    {
        _skipValues.Remove(skipValue);
        FillAllowedValues();
    }

    /// <summary>
    ///     Sets the slider's value range, step, decimal places, and unit, and updates the allowed values and visuals.
    /// </summary>
    /// <param name="currentValue">The current value to set.</param>
    /// <param name="minValue">The minimum value allowed.</param>
    /// <param name="maxValue">The maximum value allowed.</param>
    /// <param name="step">The step increment between allowed values.</param>
    /// <param name="decimalPlaces">The number of decimal places to display.</param>
    /// <param name="unit">The unit string to display (optional).</param>
    public void SetValues(T currentValue, T minValue, T maxValue, T step, int decimalPlaces, string unit = "")
    {
        _decimalPlaces = decimalPlaces;
        _currentValue = RoundValue(currentValue);
        _minValue = minValue;
        _maxValue = maxValue;
        _step = step;
        _unit = unit;
        FillAllowedValues();
        ComposeHoverTextElement();
        ComposeRestingTextElement();
        ComposeFillTexture();
    }

    /// <summary>
    ///     Sets the slider's value range, step, and unit, and updates the allowed values and visuals.
    /// </summary>
    /// <param name="currentValue">The current value to set.</param>
    /// <param name="minValue">The minimum value allowed.</param>
    /// <param name="maxValue">The maximum value allowed.</param>
    /// <param name="step">The step increment between allowed values.</param>
    /// <param name="unit">The unit string to display (optional).</param>
    public void SetValues(T currentValue, T minValue, T maxValue, T step, string unit = "")
    {
        _decimalPlaces = 0;
        _currentValue = RoundValue(currentValue);
        _minValue = minValue;
        _maxValue = maxValue;
        _step = step;
        _unit = unit;
        FillAllowedValues();
        ComposeHoverTextElement();
        ComposeRestingTextElement();
        ComposeFillTexture();
    }

    /// <summary>
    ///     Sets the current value of the slider.
    /// </summary>
    /// <param name="currentValue">The value to set.</param>
    public void SetValue(T currentValue)
        => _currentValue = RoundValue(currentValue);

    /// <summary>
    ///     The current value of the slider.
    /// </summary>
    /// <returns>The current value.</returns>
    public T GetValue()
        => _currentValue;

    /// <summary>
    ///     Disposes of all resources used by the slider, including textures and text elements.
    /// </summary>
    public override void Dispose()
    {
        GC.SuppressFinalize(this);
        base.Dispose();
        _handleTexture.Dispose();
        _hoverTextTexture.Dispose();
        _restingTextTexture.Dispose();
        _sliderFillTexture.Dispose();
        _alarmValueTexture.Dispose();
        _textElem?.Dispose();
        _textElemResting?.Dispose();
    }

    private T RoundValue(T value) => value switch
    {
        double d => (T)(object)Math.Round(d, _decimalPlaces),
        float f => (T)(object)MathF.Round(f, _decimalPlaces),
        decimal m => (T)(object)Math.Round(m, _decimalPlaces),
        _ => value
    };
}