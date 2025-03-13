using Gantry.Core.ModSystems.Abstractions;
using Vintagestory.API.Server;

namespace Gantry.Core.ModSystems;

/// <summary>
///     Acts as a base class for Universal Mod Systems, that work on both the Client, and Server.
/// </summary>
/// <seealso cref="ModSystemBase" />
public abstract class UniversalSubsystem : GantrySubsystem
{
    /// <summary>
    ///     The core API implemented by the client. The main interface for accessing the client. Contains all subcomponents, and some miscellaneous methods.
    /// </summary>
    protected ICoreClientAPI Capi { get; private set; }

    /// <summary>
    ///     The core API implemented by the server. The main interface for accessing the server. Contains all subcomponents, and some miscellaneous methods.
    /// </summary>
    protected ICoreServerAPI Sapi { get; private set; }

    /// <inheritdoc />
    public sealed override void StartPre(ICoreAPI api)
    {
        switch (api)
        {
            case ICoreClientAPI capi:
                Capi = capi;
                break;
            case ICoreServerAPI sapi:
                Sapi = sapi;
                break;
        }
        base.StartPre(api);
    }

    /// <summary>
    ///     Returns if this mod should be loaded for the given app side.
    /// </summary>
    /// <param name="forSide">For side.</param>
    /// <returns><c>true</c> if the mod should be loaded on the specified side, <c>false</c> otherwise.</returns>
    public override bool ShouldLoad(EnumAppSide forSide) => true;
}