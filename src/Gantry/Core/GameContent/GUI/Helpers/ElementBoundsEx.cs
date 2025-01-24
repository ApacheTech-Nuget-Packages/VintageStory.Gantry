namespace Gantry.Core.GameContent.GUI.Helpers;

/// <summary>
///     Extension methods to aid adding elements to GUI forms.
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class ElementBoundsEx
{
    /// <summary>
    ///     Provides an <see cref="ElementBounds"/> with a fixed size, based on the dimensions of the image file represented within the <see cref="AssetLocation"/>.
    /// </summary>
    /// <param name="imageAsset">The image asset to determine the dimensions of.</param>
    /// <param name="scale">Scales the width and height of the returned <see cref="ElementBounds"/> by the provided value.</param>
    /// <returns>An instance of <see cref="ElementBounds"/>, with a fixed size, and an origin position of (0, 0).</returns>
    public static ElementBounds ForPngImage(AssetLocation imageAsset, float scale = 1f)
    {
        if (!imageAsset.Path.EndsWith(".png"))
        {
            throw new FileLoadException("Can only determine the dimensions of a PNG file. Use https://jpg2png.com/ to quickly converts images to PNG.");
        }

        using var png = ApiEx.Client!.Assets.Get(imageAsset).ToBitmap(ApiEx.Client);
        return ElementBounds.FixedSize(png.Width * scale, png.Height * scale);
    }
}