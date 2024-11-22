using Vintagestory.API.Common;

namespace Gantry.Services.MefLab.Abstractions;

/// <summary>
///     Acts as a base class for all MefLab contracts.
/// </summary>
public abstract class MefLabContractBase : IMefLabContract
{
    /// <inheritdoc />
    public virtual void Dispose() => GC.SuppressFinalize(this);

    /// <inheritdoc />
    public abstract void Resolve(string contractName, IPlayer player, ICoreAPI api);
}