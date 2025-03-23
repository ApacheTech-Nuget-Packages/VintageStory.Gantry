using ProtoBuf;

namespace Gantry.Services.Network.Packets;

/// <summary>
///     Represents a packet containing the world name.
/// </summary>
[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
[DoNotPruneType]
public class WorldNamePacket
{
    /// <summary>
    ///     Gets or sets the name of the world.
    /// </summary>
    public string Name { get; set; }
}