using ProtoBuf;
using Vintagestory.API.MathTools;

namespace Gantry.Services.Network.Packets;

/// <summary>
///     Represents a packet for teleporting within the world map.
/// </summary>
[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
[DoNotPruneType]
public sealed class WorldMapTeleportPacket
{
    /// <summary>
    ///     Gets or sets the target position for teleportation.
    /// </summary>
    public Vec3d Position { get; set; }
}