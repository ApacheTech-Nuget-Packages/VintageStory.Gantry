using ProtoBuf;

namespace Gantry.Core.Network.Packets;

/// <summary>
///     Represents a packet containing the world name.
/// </summary>
[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
public class WorldNamePacket
{
    /// <summary>
    ///     Gets or sets the name of the world.
    /// </summary>
    public string Name { get; set; } = string.Empty;
}