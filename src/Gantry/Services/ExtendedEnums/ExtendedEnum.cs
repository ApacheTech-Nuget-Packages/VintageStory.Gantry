namespace Gantry.Services.ExtendedEnums;

/// <summary>
///     Simple implementation of a Type-based enumeration, allowing
///     equatable Type constants that can be implicitly cast to types.
/// </summary>
/// <typeparam name="TEnumType"></typeparam>
/// <typeparam name="TEnumValue"></typeparam>
/// <seealso cref="IEquatable{TEnumValue}" />
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public abstract class ExtendedEnum<TEnumType, TEnumValue> : IEquatable<TEnumValue>
    where TEnumType : class
    where TEnumValue : ExtendedEnum<TEnumType, TEnumValue>, new()
{
    /// <summary>
    ///     A dictionary of values held be this instance.
    /// </summary>
    protected static readonly Dictionary<TEnumType, TEnumValue> ValueDict = [];

    /// <summary>
    ///     The value given to this TypeEnum member.
    /// </summary>
    protected TEnumType? Value { get; init; }

    bool IEquatable<TEnumValue>.Equals(TEnumValue? other) => Value == other?.Value;

    /// <summary>
    ///     Creates the specified value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>T.</returns>
    protected static TEnumValue? Create(TEnumType value)
    {
        if (value is null) return default;
        var obj1 = new TEnumValue { Value = value };
        ValueDict.Add(value, obj1);
        return obj1;
    }

    /// <summary>
    ///     Performs an implicit conversion from <see cref="TypeEnum{T}" /> to <see cref="Type" />.
    /// </summary>
    /// <param name="enumValue">The enum value.</param>
    /// <returns>The result of the conversion.</returns>
    public static implicit operator TEnumType?(ExtendedEnum<TEnumType, TEnumValue> enumValue) => enumValue.Value;

    /// <summary>
    ///     Returns a string that represents the current object.
    /// </summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString() => Value?.ToString() ?? string.Empty;

    /// <summary>
    ///     Determines whether the specified <see cref="ExtendedEnum{TEnumType, TEnumValue}" />, is not equal to this instance.
    /// </summary>
    /// <param name="o1">The left operand of the operation.</param>
    /// <param name="o2">The right operand of the operation.</param>
    /// <returns>
    ///     Returns <c>true</c> if the left and right operands are not equal; otherwise, <c>false</c>.
    /// </returns>
    public static bool operator !=(ExtendedEnum<TEnumType, TEnumValue> o1, ExtendedEnum<TEnumType, TEnumValue> o2) => o1?.Value != o2?.Value;

    /// <summary>
    ///     Determines whether the specified <see cref="TypeEnum{T}" />, is equal to this instance.
    /// </summary>
    /// <param name="o1">The left operand of the operation.</param>
    /// <param name="o2">The right operand of the operation.</param>
    /// <returns>
    ///     Returns <c>true</c> if the left and right operands are equal; otherwise, <c>false</c>.
    /// </returns>
    public static bool operator ==(ExtendedEnum<TEnumType, TEnumValue> o1, ExtendedEnum<TEnumType, TEnumValue> o2) => o1?.Value == o2?.Value;

    /// <summary>
    ///     Determines whether the specified <see cref="object" />, is equal to this instance.
    /// </summary>
    /// <param name="other">The <see cref="object" /> to compare with this instance.</param>
    /// <returns>
    ///   <c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.
    /// </returns>
    public override bool Equals(object? other) => Value == (other as TEnumValue)?.Value;

    /// <summary>
    ///     Returns a hash code for this instance.
    /// </summary>
    /// <returns>
    /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
    /// </returns>
    public override int GetHashCode() => Value?.GetHashCode() ?? default;
}