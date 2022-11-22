using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Gantry.Core.Abstractions
{
    /// <summary>
    ///     Simple implementation of a Type-based enumeration, allowing
    ///     equatable Type constants that can be implicitly cast to types.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="IEquatable{T}" />
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public abstract class TypeEnum<T> : IEquatable<T> where T : TypeEnum<T>, new()
    {
        /// <summary>
        ///     A dictionary of values held be this instance.
        /// </summary>
        private static readonly Dictionary<Type, T> ValueDict = new();

        /// <summary>
        ///     The value given to this TypeEnum member.
        /// </summary>
        protected Type Value { get; init; }

        bool IEquatable<T>.Equals(T other)
        {
            return Value == other?.Value;
        }

        /// <summary>
        ///     Creates the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>T.</returns>
        protected static T Create(Type value)
        {
            if (value is null) return default;
            var obj1 = new T { Value = value };
            ValueDict.Add(value, obj1);
            return obj1;
        }

        /// <summary>
        ///     Creates the specified value.
        /// </summary>
        /// <typeparam name="TValue">The value.</typeparam>
        /// <returns>T.</returns>
        protected static T Create<TValue>()
        {
            var value = typeof(TValue);
            var obj1 = new T { Value = value };
            ValueDict.Add(value, obj1);
            return obj1;
        }

        /// <summary>
        ///     Performs an implicit conversion from <see cref="TypeEnum{T}" /> to <see cref="Type" />.
        /// </summary>
        /// <param name="enumValue">The enum value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Type(TypeEnum<T> enumValue)
        {
            return enumValue.Value;
        }

        /// <summary>
        ///     Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return Value.FullName;
        }

        /// <summary>
        ///     Determines whether the specified <see cref="TypeEnum{T}" />, is not equal to this instance.
        /// </summary>
        /// <param name="o1">The left operand of the operation.</param>
        /// <param name="o2">The right operand of the operation.</param>
        /// <returns>
        ///     Returns <c>true</c> if the left and right operands are not equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(TypeEnum<T> o1, TypeEnum<T> o2)
        {
            return o1?.Value != o2?.Value;
        }

        /// <summary>
        ///     Determines whether the specified <see cref="TypeEnum{T}" />, is equal to this instance.
        /// </summary>
        /// <param name="o1">The left operand of the operation.</param>
        /// <param name="o2">The right operand of the operation.</param>
        /// <returns>
        ///     Returns <c>true</c> if the left and right operands are equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(TypeEnum<T> o1, TypeEnum<T> o2)
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
            return Value == ((other as T)?.Value ?? other as Type);
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
    }
}