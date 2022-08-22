using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Gantry.Core.Abstractions
{
    /// <summary>
    ///     Simple implementation of a string-based enumeration, allowing
    ///     equatable string constants that can be implicitly cast to strings.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="IEquatable{T}" />
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public abstract class StringEnum<T> : IEquatable<T> where T : StringEnum<T>, new()
    {
        /// <summary>
        ///     A dictionary of values held be this instance.
        /// </summary>
        private static readonly Dictionary<string, T> ValueDict = new();

        /// <summary>
        ///     The value given to this StringEnum member.
        /// </summary>
        protected string Value { get; init; }

        bool IEquatable<T>.Equals(T other)
        {
            return Value.Equals(other?.Value);
        }

        /// <summary>
        ///     Creates the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>T.</returns>
        protected static T Create(string value)
        {
            if (value is null) return default;
            var obj1 = new T { Value = value };
            ValueDict.Add(value, obj1);
            return obj1;
        }

        /// <summary>
        ///     Performs an implicit conversion from <see cref="StringEnum{T}" /> to <see cref="System.String" />.
        /// </summary>
        /// <param name="enumValue">The enum value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator string(StringEnum<T> enumValue)
        {
            return enumValue.Value;
        }

        /// <summary>
        ///     Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return Value;
        }

        /// <summary>
        ///     Determines whether the specified <see cref="StringEnum{T}" />, is not equal to this instance.
        /// </summary>
        /// <param name="o1">The left operand of the operation.</param>
        /// <param name="o2">The right operand of the operation.</param>
        /// <returns>
        ///     Returns <c>true</c> if the left and right operands are not equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(StringEnum<T> o1, StringEnum<T> o2)
        {
            return o1?.Value != o2?.Value;
        }

        /// <summary>
        ///     Determines whether the specified <see cref="StringEnum{T}" />, is equal to this instance.
        /// </summary>
        /// <param name="o1">The left operand of the operation.</param>
        /// <param name="o2">The right operand of the operation.</param>
        /// <returns>
        ///     Returns <c>true</c> if the left and right operands are equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(StringEnum<T> o1, StringEnum<T> o2)
        {
            return o1?.Value == o2?.Value;
        }

        /// <summary>
        ///     Determines whether the specified <see cref="object" />, is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object other)
        {
            return Value.Equals((other as T)?.Value ?? other as string);
        }

        /// <summary>
        ///     Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        /// <summary>
        ///     Converts the string representation of the name or numeric value of one or more enumerated constants to an equivalent enumerated object.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="caseSensitive">If set to <c>true</c>, the value will be parsed as a case sensitive string. Default is False.</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static T Parse(string value, bool caseSensitive = false)
        {
            if (TryParse(value, caseSensitive, out var obj)) return obj;
            throw new InvalidOperationException(
                $"{(value == null ? "null" : $"'{value}'")} is not a valid {typeof(T).Name}");
        }

        /// <summary>
        ///     Converts the string representation of the name or numeric value of one or more enumerated constants to an equivalent enumerated object.
        ///     A parameter specifies whether the operation is case-sensitive. The return value indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="value">The string representation of the enumeration name or underlying value to convert.</param>
        /// <param name="caseSensitive"><c>false</c> to ignore case; <c>true</c> to consider case.</param>
        /// <param name="result">
        ///     When this method returns, result contains an object of type T whose value is represented by value if the parse operation succeeds.
        ///     If the parse operation fails, result contains the default value of the underlying type of T.
        ///     Note that this value need not be a member of the T enumeration.
        ///     This parameter is passed uninitialised.
        /// </param>
        /// <returns>true if the <paramref name="value">value</paramref> parameter was converted successfully; otherwise, false.</returns>
        public static bool TryParse(string value, bool caseSensitive, out T result)
        {
            result = null;
            if (value == null)
                return false;
            if (ValueDict.Count == 0)
                RuntimeHelpers.RunClassConstructor(typeof(T).TypeHandle);
            if (caseSensitive) return ValueDict.TryGetValue(value, out result);
            result = ValueDict.FirstOrDefault(f => f.Key.Equals(value, StringComparison.OrdinalIgnoreCase)).Value;
            return result is not null;
        }
    }
}