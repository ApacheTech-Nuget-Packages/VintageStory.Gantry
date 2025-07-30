using ApacheTech.Common.Extensions.Harmony;

namespace Gantry.Extensions.Harmony;

/// <summary>
///     Provides extension methods for easier and more performant use of reflection, especially for accessing base class members and properties.
/// </summary>
public static class HarmonyReflectionExtensions
{ 
    /// <summary>
    ///     Gets an array of properties within the calling instanced object, of a specified type. These can be internal or private properties within another assembly.
    /// </summary>
    /// <typeparam name="T">The type of property to return.</typeparam>
    /// <param name="instance">The instance in which the property resides.</param>
    /// <returns>An array containing the values of the properties of a specified type, reflected by this instance.</returns>
    /// <remarks>
    ///     This method is useful for extracting all properties of a given type from an object, even if they are not public.
    /// </remarks>
    public static T[] GetProperties<T>(this object instance)
    {
        return AccessTools
            .GetDeclaredProperties(instance.GetType())
            .Where(t => t.PropertyType == typeof(T))
            .Select(x => instance.GetProperty<T>(x.Name))
            .ToArray();
    }

    /// <summary>
    ///     Calls a base class method on an instance of an object via reflection.
    ///     This can be used to invoke protected or private base class methods within another assembly.
    /// </summary>
    /// <typeparam name="TBaseClass">The type of the base class.</typeparam>
    /// <param name="instance">The instance to call the base method from.</param>
    /// <param name="method">The name of the base method to call.</param>
    /// <param name="args">The arguments to pass to the method.</param>
    /// <remarks>
    ///     If the base type does not match <typeparamref name="TBaseClass"/>, the method will not be called.
    /// </remarks>
    /// <exception cref="MissingMethodException">Thrown if the base method cannot be found.</exception>
    public static void CallBaseMethod<TBaseClass>(this object instance, string method, params object[] args)
    {
        var baseType = instance.GetType().BaseType;
        if (baseType?.FullName != typeof(TBaseClass).FullName) return;
        AccessTools.Method(baseType, method)?.Invoke(instance, args);
    }

    /// <summary>
    ///     Calls a base class method on an instance of an object via reflection and returns its result.
    ///     This can be used to invoke protected or private base class methods within another assembly.
    /// </summary>
    /// <typeparam name="TBaseClass">The type of the base class.</typeparam>
    /// <typeparam name="TValue">The return type expected from the base method.</typeparam>
    /// <param name="instance">The instance to call the base method from.</param>
    /// <param name="method">The name of the base method to call.</param>
    /// <param name="args">The arguments to pass to the method.</param>
    /// <returns>The return value of the reflected base method call, or <c>default</c> if the base type does not match <typeparamref name="TBaseClass"/>.</returns>
    /// <remarks>
    ///     This method is useful for retrieving values from base class methods that are not accessible through normal means.
    /// </remarks>
    /// <exception cref="MissingMethodException">Thrown if the base method cannot be found.</exception>
    public static TValue? CallBaseMethod<TBaseClass, TValue>(this object instance, string method, params object[] args)
    {
        var baseType = instance.GetType().BaseType;
        if (baseType is not TBaseClass) return default;
        return (TValue?)AccessTools.Method(baseType, method)?.Invoke(instance, args);
    }
}