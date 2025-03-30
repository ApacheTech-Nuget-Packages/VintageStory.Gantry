using ApacheTech.Common.Extensions.Harmony;

namespace Gantry.Core.Extensions.Harmony;

/// <summary>
///     Extension methods for easier, and more performant use of reflection.
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class HarmonyReflectionExtensions
{ 
    /// <summary>
    ///     Gets an array of fields within the calling instanced object, of a specified Type. These can be an internal or private fields within another assembly.
    /// </summary>
    /// <typeparam name="T">The type of field to return.</typeparam>
    /// <param name="instance">The instance in which the field resides.</param>
    /// <returns>An array containing the values of the fields of a specified Type, reflected by this instance.</returns>
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
    /// <param name="instance">The instance to call the base method from.</param>
    /// <param name="method">The name of the base method to call.</param>
    /// <param name="args">The arguments to pass to the method.</param>
    /// <returns>The return value of the reflected base method call.</returns>
    /// <exception cref="MissingMethodException">Thrown if the base method cannot be found.</exception>
    public static void CallBaseMethod<TBaseClass>(this object instance, string method, params object[] args)
    {
        var baseType = instance.GetType().BaseType;
        if (baseType?.FullName != typeof(TBaseClass).FullName) return;
        AccessTools.Method(baseType, method)?.Invoke(instance, args);
    }

    /// <summary>
    ///     Calls a base class method on an instance of an object via reflection.
    ///     This can be used to invoke protected or private base class methods within another assembly.
    /// </summary>
    /// <typeparam name="TBaseClass">The type of the base class.</typeparam>
    /// <typeparam name="TValue">The return type expected from the base method.</typeparam>
    /// <param name="instance">The instance to call the base method from.</param>
    /// <param name="method">The name of the base method to call.</param>
    /// <param name="args">The arguments to pass to the method.</param>
    /// <returns>The return value of the reflected base method call.</returns>
    /// <exception cref="MissingMethodException">Thrown if the base method cannot be found.</exception>
    public static TValue? CallBaseMethod<TBaseClass, TValue>(this object instance, string method, params object[] args)
    {
        var baseType = instance.GetType().BaseType;
        if (baseType is not TBaseClass) return default;
        return (TValue?)AccessTools.Method(baseType, method)?.Invoke(instance, args);
    }
}