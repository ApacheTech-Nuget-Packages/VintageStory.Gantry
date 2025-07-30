namespace Gantry.Core.Abstractions.ModSystems;

/// <summary>
///     Acts as a base class for Client-Side Only ModSystems. Derived classes will only be loaded on the Client.
/// </summary>
/// <seealso cref="ModSystemBase{TModSystem}" />
public abstract class ClientModSystem<TModSystem> : ModSystemBase<TModSystem>
    where TModSystem : ClientModSystem<TModSystem>
{
    /// <summary>
    ///     The core API implemented by the client. The main interface for accessing the client. Contains all subcomponents, and some miscellaneous methods.
    /// </summary>
    protected ICoreClientAPI Capi { get; private set; } = null!;

    /// <inheritdoc />
    public sealed override void StartPre(ICoreAPI api)
    {
        if (api is not ICoreClientAPI capi) return;
        Capi = capi;
        base.StartPre(api);
    }

    /// <summary>
    ///     Returns if this mod should be loaded for the given app side.
    /// </summary>
    /// <param name="forSide">For side.</param>
    /// <returns><c>true</c> if the mod should be loaded on the specified side, <c>false</c> otherwise.</returns>
    public override bool ShouldLoad(EnumAppSide forSide) => forSide.IsClient();

    /// <inheritdoc />
    public override void StartServerSide(ICoreServerAPI api) { }

    /// <inheritdoc />
    protected sealed override void StartPreServerSide(ICoreServerAPI api) { }

    /// <inheritdoc />
    protected sealed override void StartPreUniversal(ICoreAPI api) { }

    /// <inheritdoc />
    public sealed override void AssetsFinalize(ICoreAPI api) 
        => AssetsFinalise((ICoreClientAPI)api);

    /// <inheritdoc cref="ModSystem.AssetsFinalize" />
    public virtual void AssetsFinalise(ICoreClientAPI api) 
        => base.AssetsFinalize(api);

    /// <inheritdoc />
    public sealed override void AssetsLoaded(ICoreAPI api)
        => AssetsLoaded((ICoreClientAPI)api);

    /// <inheritdoc cref="ModSystem.AssetsLoaded" />
    public virtual void AssetsLoaded(ICoreClientAPI api) 
        => base.AssetsLoaded(api);
}