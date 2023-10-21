using Cairo;
using Vintagestory.API.Client;
using Vintagestory.API.Common;

namespace Gantry.Core.GameContent.GUI.Elements;

/// <summary>
///     Represents a PNG file, that can be rendered to a GUI form. The image will stretch to fit the <see cref="ElementBounds"/> passed into the constructor.
/// </summary>
/// <seealso cref="GuiElementTextBase" />
public class GuiElementImage : GuiElementTextBase
{
    private readonly AssetLocation _imageAsset;
    private readonly Operator _blendMode;

    /// <summary>
    /// 	Initialises a new instance of the <see cref="GuiElementImage"/> class.
    /// </summary>
    /// <param name="capi">The capi.</param>
    /// <param name="bounds">The bounds.</param>
    /// <param name="imageAsset">The image asset.</param>
    /// <param name="blendMode"></param>
    public GuiElementImage(ICoreClientAPI capi, ElementBounds bounds, AssetLocation imageAsset, Operator blendMode)
        : base(capi, "", null, bounds)
    {
        _imageAsset = imageAsset;
        _blendMode = blendMode;
        RenderAsPremultipliedAlpha = true;
    }

    /// <summary>
    ///     Draws the image to the current context.
    /// </summary>
    /// <param name="context">The context to draw with.</param>
    /// <param name="originalSurface">The current surface used by the context. This will not be written to directly.</param>
    public override void ComposeElements(Context context, ImageSurface originalSurface)
    {
        context.Save();
        var originalBlendMode = context.Operator;
        context.Operator = _blendMode;

        using var imageSurface = getImageSurfaceFromAsset(api, _imageAsset);
        var scaleX = Bounds.OuterWidth / imageSurface.Width;
        var scaleY = Bounds.OuterHeight / imageSurface.Height;

        context.SetSourceRGBA(0,0,0,0);
        context.SetSource(imageSurface);
        context.Scale(scaleX, scaleY);
        imageSurface.Show(context, Bounds.drawX / scaleX, Bounds.drawY / scaleY);

        context.Operator = originalBlendMode;
        context.Restore();
    }
}