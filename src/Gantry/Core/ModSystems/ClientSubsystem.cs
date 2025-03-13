using Gantry.Core.ModSystems.Abstractions;
using Vintagestory.API.Server;

namespace Gantry.Core.ModSystems;

/// <summary>
///     Acts as a base class for Client-Side Only ModSystems. Derived classes will only be loaded on the Client.
/// </summary>
/// <seealso cref="ModSystemBase" />
public abstract class ClientSubsystem : GantrySubsystem
{
    /// <summary>
    ///     The core API implemented by the client. The main interface for accessing the client. Contains all subcomponents, and some miscellaneous methods.
    /// </summary>
    protected ICoreClientAPI Capi { get; private set; }

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
    public sealed override void StartPreUniversal(ICoreAPI api)
        => base.StartPreUniversal(api);

    /// <inheritdoc />
    public sealed override void StartPreServerSide(ICoreServerAPI sapi)
        => base.StartPreServerSide(sapi);

    /// <inheritdoc />
    public sealed override void StartServerSide(ICoreServerAPI sapi)
        => base.StartServerSide(sapi);

    /// <inheritdoc />
    public sealed override void ConfigureUniversalModServices(IServiceCollection services, ICoreAPI api)
        => base.ConfigureUniversalModServices(services, api);

    /// <inheritdoc />
    public sealed override void ConfigureServerModServices(IServiceCollection services, ICoreServerAPI sapi) 
        => base.ConfigureServerModServices(services, sapi);
}