namespace Gantry.GameContent.GUI.Helpers;

/// <summary>
///     Extension methods to aid adding elements to GUI forms.
/// </summary>
public static class ElementBoundsEx
{
    /// <summary>
    ///     Provides an <see cref="ElementBounds"/> with a fixed size, based on the dimensions of the image file represented within the <see cref="AssetLocation"/>.
    /// </summary>
    /// <param name="imageAsset">The image asset to determine the dimensions of.</param>
    /// <param name="capi">The client API to use to access the image asset.</param>
    /// <param name="scale">Scales the width and height of the returned <see cref="ElementBounds"/> by the provided value.</param>
    /// <returns>An instance of <see cref="ElementBounds"/>, with a fixed size, and an origin position of (0, 0).</returns>
    public static ElementBounds ForPngImage(AssetLocation imageAsset, ICoreClientAPI capi, float scale = 1f)
    {
        if (!imageAsset.Path.EndsWith(".png"))
        {
            throw new FileLoadException("Can only determine the dimensions of a PNG file. Use https://jpg2png.com/ to quickly converts images to PNG.");
        }

        using var png = capi.Assets.Get(imageAsset).ToBitmap(capi);
        return ElementBounds.FixedSize(png.Width * scale, png.Height * scale);
    }
}