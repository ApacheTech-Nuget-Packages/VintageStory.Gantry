using ProtoBuf;

namespace Gantry.Services.Network.Packets;

/// <summary>
///     Sets a specific value within an instance.
/// </summary>
[ProtoContract]
public class SetPropertyPacket<T> : SetPropertyPacketBase
{
    /// <summary>
    ///     The value to set.
    /// </summary>
    [ProtoMember(1)]
    public new T Value { get; set; }

    /// <summary>
    ///     The boxable value to set.
    /// </summary>
    protected override object UntypedValue { get => Value; set => Value = (T)value; }

    /// <inheritdoc />
    public override string ToString() => Value?.ToString() ?? "";
}