using ApacheTech.Common.Extensions.Harmony;
using HarmonyLib;
using JetBrains.Annotations;

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
}