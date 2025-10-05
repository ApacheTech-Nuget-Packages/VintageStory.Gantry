using ProtoBuf;

namespace Gantry.Core.Network.Packets;

/// <summary>
///     Represents a packet containing the world name.
/// </summary>
[ProtoContract]
public class RequestPacket
{
    /// <summary>
    ///     Gets or sets the name of the world.
    /// </summary>
    [ProtoMember(1)]
    public required RequestType Request { get; set; }
}