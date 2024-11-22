using Vintagestory.API.Common;

namespace Gantry.Services.MefLab.Abstractions;

/// <summary>
///     Defines the entry-point for a MefLab contract.
/// </summary>
public interface IMefLabContract : IDisposable
{
    /// <summary>
    ///     Resolves the contract.
    /// </summary>
    void Resolve(string contractName, IPlayer player, ICoreAPI api);

    /// <summary>
    ///     The id used to resolve MefLab contracts.
    /// </summary>
    const string ContractId = "CompositionData";
}