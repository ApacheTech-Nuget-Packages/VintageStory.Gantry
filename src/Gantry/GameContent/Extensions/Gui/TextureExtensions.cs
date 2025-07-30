namespace Gantry.GameContent.Extensions.Gui;

/// <summary>
///     Extension methods to aid when working with textures.
/// </summary>
public static class LoadedTextureExtensions
{
    /// <summary>
    ///     Deletes the specified <see cref="LoadedTexture"/>. Can only be run on the Client.
    /// </summary>
    /// <param name="texture">The texture to delete.</param>
    /// <param name="capi">The client API.</param>
    public static void Delete(this LoadedTexture texture, ICoreClientAPI capi)
    {
        capi.Gui.DeleteTexture(texture.TextureId);
    }
}