using ProtoBuf;

namespace Gantry.Services.Network.Packets;

/// <summary>
///     Sets a specific value within an instance.
/// </summary>
[ProtoContract]
[ProtoInclude(03, typeof(SetPropertyPacket<bool>))]
[ProtoInclude(04, typeof(SetPropertyPacket<byte>))]
[ProtoInclude(05, typeof(SetPropertyPacket<sbyte>))]
[ProtoInclude(06, typeof(SetPropertyPacket<char>))]
[ProtoInclude(07, typeof(SetPropertyPacket<decimal>))]
[ProtoInclude(08, typeof(SetPropertyPacket<double>))]
[ProtoInclude(09, typeof(SetPropertyPacket<float>))]
[ProtoInclude(10, typeof(SetPropertyPacket<int>))]
[ProtoInclude(11, typeof(SetPropertyPacket<uint>))]
[ProtoInclude(14, typeof(SetPropertyPacket<long>))]
[ProtoInclude(15, typeof(SetPropertyPacket<ulong>))]
[ProtoInclude(16, typeof(SetPropertyPacket<short>))]
[ProtoInclude(17, typeof(SetPropertyPacket<ushort>))]
[ProtoInclude(18, typeof(SetPropertyPacket<string>))]
[ProtoInclude(19, typeof(SetPropertyPacket<dynamic>))]
[ProtoInclude(20, typeof(SetPropertyPacket<>))]
public abstract class SetPropertyPacketBase
{
    /// <summary>
    ///     The value to set.
    /// </summary>
    public object Value
    {
        get => UntypedValue;
        set => UntypedValue = value;
    }

    /// <summary>
    ///     The name of the property to set.
    /// </summary>
    [ProtoMember(2)]
    public string PropertyName { get; set; }

    /// <summary>
    ///     The boxable value to set.
    /// </summary>
    protected abstract object UntypedValue { get; set; }

    /// <summary>
    ///     Initialises a new instance of the <see cref="SetPropertyPacket{T}"/> class.
    /// </summary>
    /// <typeparam name="T">The type of the value to set.</typeparam>
    /// <param name="value">The value to set.</param>
    public static SetPropertyPacket<T> Create<T>(T value) => new() { Value = value };
}