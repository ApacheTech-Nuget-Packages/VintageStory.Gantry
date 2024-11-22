using System.ComponentModel.Composition;
using Gantry.Services.MefLab.Extensions;
using Vintagestory.API.Common;

namespace Gantry.Services.MefLab.Abstractions;

/// <summary>
///     
/// </summary>
public abstract class MefLabSystem
{
    /// <summary>
    ///     Gets or sets the data packet used for MEF composition.
    /// </summary>
    /// <value>The data packet used for MEF composition.</value>
    [Import(IMefLabContract.ContractId, AllowRecomposition = true)]
    private IMefLabContract Contract { get; set; }


    /// <inheritdoc />
    protected void ResolveContract(CompositionDataPacket packet, IPlayer player, ICoreAPI api)
    {
        try
        {
            var container = packet.Containerise();
            container.ComposeParts(this);
            Contract?.Resolve(packet.Contract, player, api);
        }
        finally
        {
            Contract?.Dispose();
        }
    }
}
