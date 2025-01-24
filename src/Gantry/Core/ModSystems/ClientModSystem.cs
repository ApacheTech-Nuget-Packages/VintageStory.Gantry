using Gantry.Core.ModSystems.Abstractions;
using Vintagestory.API.Server;

namespace Gantry.Core.ModSystems;

/// <summary>
///     Acts as a base class for Client-Side Only ModSystems. Derived classes will only be loaded on the Client.
/// </summary>
/// <seealso cref="ModSystemBase" />
public abstract class ClientModSystem : ModSystemBase
{
    /// <summary>
    ///     The core API implemented by the client. The main interface for accessing the client. Contains all subcomponents, and some miscellaneous methods.
    /// </summary>
    protected static ICoreClientAPI Capi => UApi as ICoreClientAPI;

    /// <summary>
    ///     Returns if this mod should be loaded for the given app side.
    /// </summary>
    /// <param name="forSide">For side.</param>
    /// <returns><c>true</c> if the mod should be loaded on the specified side, <c>false</c> otherwise.</returns>
    public override bool ShouldLoad(EnumAppSide forSide)
    {
        return forSide.IsClient();
    }

    /// <inheritdoc />
    public override void StartServerSide(ICoreServerAPI api) { }

    /// <inheritdoc />
    protected sealed override void StartPreServerSide(ICoreServerAPI api) { }

    /// <inheritdoc />
    protected sealed override void StartPreUniversal(ICoreAPI api) { }

    /// <inheritdoc />
    public sealed override void AssetsFinalize(ICoreAPI api) 
    { 
        AssetsFinalise(api as ICoreClientAPI);
    }

    /// <inheritdoc cref="ModSystem.AssetsFinalize" />
    public virtual void AssetsFinalise(ICoreClientAPI api)
    {
        base.AssetsFinalize(api);
    }

    /// <inheritdoc />
    public sealed override void AssetsLoaded(ICoreAPI api)
    {
        AssetsLoaded(api as ICoreClientAPI);
    }

    /// <inheritdoc cref="ModSystem.AssetsLoaded" />
    public virtual void AssetsLoaded(ICoreClientAPI api)
    {
        base.AssetsLoaded(api);
    }
}