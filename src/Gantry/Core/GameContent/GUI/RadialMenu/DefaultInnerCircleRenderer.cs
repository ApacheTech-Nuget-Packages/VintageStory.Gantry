using Cairo;
using Gantry.Core.GameContent.GUI.RadialMenu.Abstractions;

namespace Gantry.Core.GameContent.GUI.RadialMenu;

/// <summary>
///    Represents a radial element with a position and associated events.
/// </summary>
public class DefaultInnerCircleRenderer : IInnerCircleRenderer, IDisposable
{
    private readonly int _radius = 1;
    private readonly int _gape = -1;
    private readonly ICoreClientAPI _api;
    private readonly double[] _fillColor;
    private LoadedTexture _texture;
    private readonly double[] _circleColor;
    private readonly int _lineWidth = 6;
    private readonly IRenderAPI _renderer;
    private int _halfTextureSize = -1;
    private int _textureSize;
    private string _text;
    private LoadedTexture _textTexture;
    private readonly CairoFont _font;

    /// <summary>
    ///     Initialises a new instance of the <see cref="DefaultInnerCircleRenderer"/> class.
    /// </summary>
    /// <param name="api">The API used for rendering.</param>
    /// <param name="lineWidth">The line width for the circle.</param>
    public DefaultInnerCircleRenderer(ICoreClientAPI api, int lineWidth)
    {
        TextTextureUtil = new TextTextureUtil(api);
        _api = api;
        _renderer = api.Render;
        _fillColor = GuiStyle.DialogLightBgColor;
        _circleColor = GuiStyle.DialogLightBgColor;
        _texture = new LoadedTexture(api);
        _font = new CairoFont(GuiStyle.NormalFontSize, GuiStyle.StandardFontName)
        {
            Orientation = (EnumTextOrientation)2,
            Color = GuiStyle.DialogDefaultTextColor
        };
    }

    /// <summary>
    ///     Gets the text texture utility for rendering text.
    /// </summary>
    protected TextTextureUtil TextTextureUtil { get; private set; }

    /// <summary>
    ///     Gets or sets the radius of the circle.
    /// </summary>
    public int Radius { get; set; } = 1;

    /// <summary>
    ///     Gets or sets the gape value for the circle.
    /// </summary>
    public int Gape { get; set; } = -1;

    /// <summary>
    ///     Gets or sets the text to be displayed in the circle.
    /// </summary>
    public string DisplayedText
    {
        get => _text;
        set
        {
            _text = value;
            RebuildText();
        }
    }

    /// <summary>
    ///     Rebuilds the circle and its texture.
    /// </summary>
    public void Rebuild() => RebuildMiddleCircle();

    /// <summary>
    ///     Rebuilds the text texture.
    /// </summary>
    public void RebuildText()
    {
        if (_text == null)
        {
            if (_textTexture?.Disposed ?? true)
                return;
            _textTexture.Dispose();
            _textTexture = null;
        }
        else
        {
            if (_textTexture == null)
                _textTexture = new LoadedTexture(_api);

            var strArray = _text.Split('\n');
            var length = strArray.Length;
            var fontSize = (float)(_font.UnscaledFontsize * 1.05);
            var textureSize = _textureSize;
            var num2 = (float)((textureSize - length * (double)fontSize) / 2.0 + length * (double)fontSize / 2.0);

            var imageSurface = new ImageSurface(0, textureSize, textureSize);
            var context = new Context(imageSurface);
            _font.SetupContext(context);

            for (var index = 0; index < length; ++index)
            {
                var str = strArray[index];
                var textExtents = _font.GetTextExtents(str);
                var xadvance = (float)textExtents.XAdvance;
                context.MoveTo((textureSize - (double)xadvance) / 2.0, (double)num2 + index * (double)fontSize);
                context.ShowText(str);
            }
            _api.Gui.LoadOrUpdateCairoTexture(imageSurface, true, ref _textTexture);

            context.Dispose();
            imageSurface.Dispose();
        }
    }

    /// <summary>
    ///     Rebuilds the middle circle.
    /// </summary>
    public void RebuildMiddleCircle()
    {
        var size = (_radius - _gape) * 2;
        _textureSize = size;
        _halfTextureSize = size / 2;

        var imageSurface = new ImageSurface(0, size, size);
        var context = new Context(imageSurface);
        context.SetSourceRGBA(_fillColor[0], _fillColor[1], _fillColor[2], _fillColor.Length > 3 ? _fillColor[3] : 0.3);
        context.LineWidth = 6.0;
        var num2 = size / 2.0;
        context.Arc(num2, num2, _radius - _gape, 0.0, 6.28318548202515);
        context.ClosePath();
        context.Fill();
        context.LineWidth = _lineWidth;
        context.SetSourceRGBA(_circleColor[0], _circleColor[1], _circleColor[2], _fillColor.Length > 3 ? _circleColor[3] : 1.0);
        context.Stroke();
        _api.Gui.LoadOrUpdateCairoTexture(imageSurface, true, ref _texture);

        context.Dispose();
        imageSurface.Dispose();
    }

    /// <summary>
    ///     Renders the inner circle and its text at the specified coordinates.
    /// </summary>
    /// <param name="x">The x-coordinate for rendering.</param>
    /// <param name="y">The y-coordinate for rendering.</param>
    public void Render(int x, int y)
    {
        if (_texture?.Disposed == false)
            _renderer.Render2DLoadedTexture(_texture, x - _halfTextureSize, y - _halfTextureSize, 50f);

        if (_textTexture?.Disposed == false)
            _renderer.Render2DTexture(_textTexture.TextureId, x - _halfTextureSize, y - _halfTextureSize, _textureSize, _textureSize, 50f);
    }

    /// <summary>
    ///     Disposes of the textures used by the renderer.
    /// </summary>
    public void Dispose()
    {
        _texture?.Dispose();
        _textTexture?.Dispose();
        GC.SuppressFinalize(this);
    }
}