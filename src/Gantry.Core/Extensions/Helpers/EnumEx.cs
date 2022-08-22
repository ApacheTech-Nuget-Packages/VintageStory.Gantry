using JetBrains.Annotations;

namespace Gantry.Core.Extensions.Helpers
{
    /// <summary>
    ///     Helper class that extends the normal functionality of enums.
    /// </summary>
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public static class EnumEx
    {
        /// <summary>
        ///     Converts the string representation of the name or numeric value of one or more enumerated constants to an
        ///     equivalent enumerated object. A parameter specifies whether the operation is case-insensitive.
        /// </summary>
        /// <typeparam name="TEnum">An enumeration type.</typeparam>
        /// <param name="value">A string containing the name or value to convert.</param>
        /// <param name="ignoreCase">true to ignore case; false to regard case.</param>
        /// <returns>
        ///     An object of type <typeparamref name="TEnum">enumType</typeparamref> whose value is represented by
        ///     <paramref name="value">value</paramref>.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <typeparamref name="TEnum">enumType</typeparamref> or
        ///     <paramref name="value">value</paramref> is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        ///     <typeparamref name="TEnum">enumType</typeparamref> is not an
        ///     <see cref="T:System.Enum"></see>.   -or-  <paramref name="value">value</paramref> is either an empty string ("") or
        ///     only contains white space.   -or-  <paramref name="value">value</paramref> is a name, but not one of the named
        ///     constants defined for the enumeration.
        /// </exception>
        /// <exception cref="T:System.OverflowException">
        ///     <paramref name="value">value</paramref> is outside the range of the
        ///     underlying type of <typeparamref name="TEnum">enumType</typeparamref>.
        /// </exception>
        public static TEnum Parse<TEnum>(string value, bool ignoreCase = false) where TEnum : System.Enum
        {
            return (TEnum)System.Enum.Parse(typeof(TEnum), value, ignoreCase);
        }
    }
}