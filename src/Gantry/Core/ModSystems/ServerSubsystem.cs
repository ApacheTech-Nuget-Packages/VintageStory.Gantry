using Gantry.Core.ModSystems.Abstractions;
using Vintagestory.API.Server;

namespace Gantry.Core.ModSystems;

/// <summary>
///     Acts as a base class for Server-Side Only ModSystems. Derived classes will only be loaded on the Server.
/// </summary>
/// <seealso cref="ModSystemBase" />
public abstract class ServerSubsystem : GantrySubsystem
{
    /// <summary>
    ///     The core API implemented by the server. The main interface for accessing the server. Contains all subcomponents, and some miscellaneous methods.
    /// </summary>
    protected static ICoreServerAPI Sapi => UApi as ICoreServerAPI;

    /// <summary>
    ///     Returns if this mod should be loaded for the given app side.
    /// </summary>
    /// <param name="forSide">For side.</param>
    /// <returns><c>true</c> if the mod should be loaded on the specified side, <c>false</c> otherwise.</returns>
    public override bool ShouldLoad(EnumAppSide forSide)
    {
        return forSide.IsServer();
    }

    /// <inheritdoc />
    public sealed override void StartPreUniversal(ICoreAPI api)
        => base.StartPreUniversal(api);

    /// <inheritdoc />
    public sealed override void StartPreClientSide(ICoreClientAPI capi)
        => base.StartPreClientSide(capi);

    /// <inheritdoc />
    public sealed override void StartClientSide(ICoreClientAPI capi)
        => base.StartClientSide(capi);

    /// <inheritdoc />
    public sealed override void ConfigureUniversalModServices(IServiceCollection services, ICoreAPI api) 
        => base.ConfigureUniversalModServices(services, api);

    /// <inheritdoc />
    public sealed override void ConfigureClientModServices(IServiceCollection services, ICoreClientAPI capi) 
        => base.ConfigureClientModServices(services, capi);
}