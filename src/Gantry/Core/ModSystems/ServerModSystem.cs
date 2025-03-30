using Gantry.Core.ModSystems.Abstractions;
using Vintagestory.API.Server;

namespace Gantry.Core.ModSystems;

/// <summary>
///     Acts as a base class for Server-Side Only ModSystems. Derived classes will only be loaded on the Server.
/// </summary>
/// <seealso cref="ModSystemBase" />
public abstract class ServerModSystem : ModSystemBase
{
    /// <summary>
    ///     The core API implemented by the server. The main interface for accessing the server. Contains all subcomponents, and some miscellaneous methods.
    /// </summary>
    protected ICoreServerAPI Sapi { get; private set; } = null!;

    /// <inheritdoc />
    public sealed override void StartPre(ICoreAPI api)
    {
        if (api is not ICoreServerAPI sapi) return;
        Sapi = sapi;
        base.StartPre(api);
    }

    /// <summary>
    ///     Returns if this mod should be loaded for the given app side.
    /// </summary>
    /// <param name="forSide">For side.</param>
    /// <returns><c>true</c> if the mod should be loaded on the specified side, <c>false</c> otherwise.</returns>
    public override bool ShouldLoad(EnumAppSide forSide) => forSide.IsServer();

    /// <inheritdoc />
    public sealed override void StartClientSide(ICoreClientAPI api) { }

    /// <inheritdoc />
    protected sealed override void StartPreClientSide(ICoreClientAPI capi) { }

    /// <inheritdoc />
    protected sealed override void StartPreUniversal(ICoreAPI api) { }

    /// <inheritdoc />
    public override void AssetsFinalize(ICoreAPI api)
    {
        AssetsFinalise((ICoreServerAPI)api);
    }

    /// <inheritdoc cref="ModSystem.AssetsFinalize" />
    public virtual void AssetsFinalise(ICoreServerAPI api)
    {
        base.AssetsFinalize(api);
    }

    /// <inheritdoc />
    public sealed override void AssetsLoaded(ICoreAPI api)
    {
        AssetsLoaded((ICoreServerAPI)api);
    }

    /// <inheritdoc cref="ModSystem.AssetsLoaded" />
    public virtual void AssetsLoaded(ICoreServerAPI api)
    {
        base.AssetsLoaded(api);
    }
}