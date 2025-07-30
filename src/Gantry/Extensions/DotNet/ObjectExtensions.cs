#nullable enable

namespace Gantry.Extensions.DotNet;

/// <summary>
///     Extension methods to aid working with objects.
/// </summary>
public static class ObjectExtensions
{
    /// <summary>
    ///     Copies the properties from the source object into the target object.
    ///     Only properties that have both a public getter and setter will be copied.
    /// </summary>
    /// <param name="target">The target object where the properties will be copied to.</param>
    /// <param name="source">The source object from which the properties will be copied.</param>
    public static void CopyFrom<TBase, TTo, TFrom>(this TTo target, TFrom source)
        where TFrom : TBase
        where TTo : TBase
    {
        if (target == null) throw new ArgumentNullException(nameof(target));
        if (source == null) throw new ArgumentNullException(nameof(source));

        // Get all public properties from the type T
        var properties = typeof(TBase).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var property in properties)
        {
            // Ensure the property has both a public getter and setter
            if (property.CanRead && property.CanWrite)
            {
                // Get the value from the source object
                var value = property.GetValue(source);

                // Set the value to the target object
                property.SetValue(target, value);
            }
        }
    }

    /// <summary>
    ///     Copies the properties from the source object into the target object.
    ///     Only properties that have both a public getter and setter will be copied.
    /// </summary>
    /// <param name="target">The target object where the properties will be copied to.</param>
    /// <param name="source">The source object from which the properties will be copied.</param>
    public static void CopyFrom<TDerived, TBase>(this TDerived target, TBase source)
        where TDerived : TBase
    {
        if (target == null) throw new ArgumentNullException(nameof(target));
        if (source == null) throw new ArgumentNullException(nameof(source));

        // Get all public properties from the type T
        var properties = typeof(TBase).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var property in properties)
        {
            // Ensure the property has both a public getter and setter
            if (property.CanRead && property.CanWrite)
            {
                // Get the value from the source object
                var value = property.GetValue(source);

                // Set the value to the target object
                property.SetValue(target, value);
            }
        }
    }

    /// <summary>
    ///     Creates a new instance, and copies the properties from the source object into the target object.
    ///     Only properties that have both a public getter and setter will be copied.
    /// </summary>
    /// <param name="source">The source object from which the properties will be copied.</param>
    public static TDerived CreateFrom<TBase, TDerived>(this TBase source)
        where TDerived : TBase, new()
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        var target = new TDerived();

        // Get all public properties from the type T
        var properties = typeof(TBase).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var property in properties)
        {
            // Ensure the property has both a public getter and setter
            if (property.CanRead && property.CanWrite)
            {
                // Get the value from the source object
                var value = property.GetValue(source);

                // Set the value to the target object
                property.SetValue(target, value);
            }
        }
        
        return target;
    }



    /// <summary>
    ///     Dynamically safe-casts the object instance to a specified type.
    /// </summary>
    /// <typeparam name="T">The type of object to safe-cast to.</typeparam>
    /// <param name="obj">The instance to safe-cast.</param>
    /// <returns>An instance of Type <typeparamref name="T" />.</returns>
    public static T As<T>(this object obj)
    {
        return Type.GetTypeCode(typeof(T)) is TypeCode.DateTime or TypeCode.DBNull or TypeCode.Empty
            ? throw new ArgumentOutOfRangeException(nameof(T),
                "Objects of this TypeCode cannot be cast to, dynamically.")
            : obj is T t ? t : default!;
    }
}