using ProtoBuf;

namespace Gantry.Services.Network.Packets;

/// <summary>
///     Sets a specific value within an instance.
/// </summary>
[ProtoContract]
[DoNotPruneType]
public class SetPropertyPacket<T> : SetPropertyPacketBase where T : notnull
{
    /// <summary>
    ///     The value to set.
    /// </summary>
    [ProtoMember(1)]
    public new T Value { get; set; } = default!;

    /// <summary>
    ///     The boxable value to set.
    /// </summary>
    protected override object UntypedValue { get => Value; set => Value = (T)value; }

    /// <inheritdoc />
    public override string ToString() => Value?.ToString() ?? "";
}