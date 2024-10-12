namespace Gantry.Services.ExtendedEnums;

/// <summary>
///     Represents a primitive, build-in type, within C#.
/// </summary>
/// <seealso cref="TypeEnum{PrimitiveType}" />
public class PrimitiveType : TypeEnum<PrimitiveType>
{
    /// <summary>
    ///     A <see cref="bool"/> value.
    /// </summary>
    public static PrimitiveType Boolean { get; } = Create<bool>();

    /// <summary>
    ///     A <see cref="byte"/> value.
    /// </summary>
    public static PrimitiveType Byte { get; } = Create<byte>();

    /// <summary>
    ///     A <see cref="sbyte"/> value.
    /// </summary>
    public static PrimitiveType SignedByte { get; } = Create<sbyte>();

    /// <summary>
    ///     A <see cref="short"/> value.
    /// </summary>
    public static PrimitiveType Short { get; } = Create<short>();

    /// <summary>
    ///     A <see cref="ushort"/> value.
    /// </summary>
    public static PrimitiveType UnsignedShort { get; } = Create<ushort>();

    /// <summary>
    ///     A <see cref="int"/> value.
    /// </summary>
    public static PrimitiveType Integer { get; } = Create<int>();

    /// <summary>
    ///     A <see cref="uint"/> value.
    /// </summary>
    public static PrimitiveType UnsignedInteger { get; } = Create<uint>();

    /// <summary>
    ///     A <see cref="nint"/> value.
    /// </summary>
    public static PrimitiveType NativeInteger { get; } = Create<nint>();

    /// <summary>
    ///     A <see cref="nuint"/> value.
    /// </summary>
    public static PrimitiveType UnsignedNativeInteger { get; } = Create<nuint>();

    /// <summary>
    ///     A <see cref="long"/> value.
    /// </summary>
    public static PrimitiveType Long { get; } = Create<long>();

    /// <summary>
    ///     A <see cref="ulong"/> value.
    /// </summary>
    public static PrimitiveType UnsignedLong { get; } = Create<ulong>();

    /// <summary>
    ///     A <see cref="float"/> value.
    /// </summary>
    public static PrimitiveType Float { get; } = Create<float>();

    /// <summary>
    ///     A <see cref="double"/> value.
    /// </summary>
    public static PrimitiveType Double { get; } = Create<double>();

    /// <summary>
    ///     A <see cref="decimal"/> value.
    /// </summary>
    public static PrimitiveType Decimal { get; } = Create<decimal>();

    /// <summary>
    ///     A <see cref="char"/> value.
    /// </summary>
    public static PrimitiveType Char { get; } = Create<char>();

    /// <summary>
    ///     A <see cref="string"/> value.
    /// </summary>
    public static PrimitiveType String { get; } = Create<string>();
}