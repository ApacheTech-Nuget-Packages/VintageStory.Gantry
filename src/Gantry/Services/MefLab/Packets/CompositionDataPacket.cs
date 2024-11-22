using ProtoBuf;

namespace Gantry.Services.MefLab;

/// <summary>
///     Represents a network packet used for MEF composition.
/// </summary>
[ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
public class CompositionDataPacket
{
    /// <summary>
    ///     The name of the contract.
    /// </summary>
    public string Contract { get; set; }

    /// <summary>
    ///     The data associated with the contract.
    /// </summary>
    public IEnumerable<byte> Data { get; set; }
}