using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

namespace Gantry.Core.Diagnostics
{
    /// <summary>
    ///     Guard clauses to streamline error checking of input parameters.
    /// </summary>
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public static class Guard
    {
        /// <summary>
        ///     Throws an <see cref="ArgumentNullException"/> if the specified <see cref="object"/> is <see langword="null"/>.
        /// </summary>
        /// <param name="argumentName">The name of the object to check.</param>
        /// <param name="value">The object to check.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void AgainstNull(string argumentName, object value)
        {
            if (value is not null) return;
            throw new ArgumentNullException(argumentName);
        }

        /// <summary>
        ///     Throws an <see cref="ArgumentException"/> if the specified <see cref="Type"/> does not have a default constructor.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="argumentName">The name of the object to check.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void TypeHasDefaultConstructor(Type type, string argumentName)
        {
            if (type.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Any(ctor => ctor.GetParameters().Length == 0)) return;
            var error = $"Type '{type.FullName}' must have a default constructor.";
            throw new ArgumentException(error, argumentName);
        }

        /// <summary>
        ///     Throws an <see cref="ArgumentNullException"/> if the specified <see cref="string"/> is <see langword="null"/> or empty.
        /// </summary>
        /// <param name="argumentName">The name of the object to check.</param>
        /// <param name="value">The object to check.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void AgainstNullAndEmpty(string argumentName, string value)
        {
            if (!string.IsNullOrWhiteSpace(value)) return;
            throw new ArgumentNullException(argumentName);
        }

        /// <summary>
        ///     Throws an <see cref="ArgumentNullException"/> if the specified <see cref="ICollection"/> is <see langword="null"/>.
        ///     Throws an <see cref="ArgumentOutOfRangeException"/> if the specified <see cref="ICollection"/> is empty.
        /// </summary>
        /// <param name="argumentName">The name of the object to check.</param>
        /// <param name="value">The object to check.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void AgainstNullAndEmpty(string argumentName, ICollection value)
        {
            if (value is null)
            {
                throw new ArgumentNullException(argumentName);
            }
            if (value.Count == 0)
            {
                throw new ArgumentOutOfRangeException(argumentName);
            }
        }

        /// <summary>
        ///     Throws an <see cref="ArgumentOutOfRangeException"/> if the specified <see cref="int"/> value is less than or equal to zero.
        /// </summary>
        /// <param name="argumentName">The name of the object to check.</param>
        /// <param name="value">The object to check.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void AgainstNegativeAndZero(string argumentName, int value)
        {
            if (value <= 0)
            {
                throw new ArgumentOutOfRangeException(argumentName);
            }
        }

        /// <summary>
        ///     Throws an <see cref="ArgumentOutOfRangeException"/> if the specified <see cref="int"/> value is less than zero.
        /// </summary>
        /// <param name="argumentName">The name of the object to check.</param>
        /// <param name="value">The object to check.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void AgainstNegative(string argumentName, int value)
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(argumentName);
            }
        }

        /// <summary>
        ///     Throws an <see cref="ArgumentOutOfRangeException"/> if the specified <see cref="TimeSpan"/> is less than or equal to zero.
        /// </summary>
        /// <param name="argumentName">The name of the object to check.</param>
        /// <param name="value">The object to check.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void AgainstNegativeAndZero(string argumentName, TimeSpan value)
        {
            if (value <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(argumentName);
            }
        }

        /// <summary>
        ///     Throws an <see cref="ArgumentOutOfRangeException"/> if the specified <see cref="TimeSpan"/> is less than zero.
        /// </summary>
        /// <param name="argumentName">The name of the object to check.</param>
        /// <param name="value">The object to check.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void AgainstNegative(string argumentName, TimeSpan value)
        {
            if (value < TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(argumentName);
            }
        }
    }
}