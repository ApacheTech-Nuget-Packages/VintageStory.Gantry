namespace Gantry.Services.ExtendedEnums;

/// <summary>
///     Simple implementation of a Type-based enumeration, allowing
///     equatable Type constants that can be implicitly cast to types.
/// </summary>
/// <typeparam name="T"></typeparam>
/// <seealso cref="IEquatable{T}" />
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public abstract class TypeEnum<T> : ExtendedEnum<Type, T>
    where T : TypeEnum<T>, new()
{
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
    ///     Returns a string that represents the current object.
    /// </summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString()
    {
        return Value?.FullName ?? string.Empty;
    }
}