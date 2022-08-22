using JetBrains.Annotations;
using Vintagestory.API.Client;
using Vintagestory.API.Common;

namespace Gantry.Core.Extensions.GameContent.Gui
{
    /// <summary>
    ///     Extension methods to aid when working with textures.
    /// </summary>
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public static class LoadedTextureExtensions
    {
        /// <summary>
        ///     Deletes the specified <see cref="LoadedTexture"/>. Can only be run on the Client.
        /// </summary>
        /// <param name="texture">The texture to delete.</param>
        public static void Delete(this LoadedTexture texture)
        {
            if (ApiEx.Side.IsServer()) return;
            ApiEx.Client.Gui.DeleteTexture(texture.TextureId);
        }
    }
}