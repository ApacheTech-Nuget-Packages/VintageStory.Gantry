using ProtoBuf;

namespace Gantry.Core.Network.Packets;

/// <summary>
///     Represents a generic signalling packet, used to raise an event for a change of state.
/// </summary>
[ProtoContract]
public sealed class SignalPacket
{
    /// <summary>
    ///     Initialises an instance of the <see cref="SignalPacket"/> class.
    /// </summary>
    public static SignalPacket Ping { get; } = new();
        
}