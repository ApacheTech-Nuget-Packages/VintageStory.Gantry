using System.Dynamic;

namespace Gantry.Extensions.DotNet;

/// <summary>
///     Helper Methods for working with <see cref="ExpandoObject"/>s and <c>dynamic</c> objects.
/// </summary>
public static class DynamicExtensions
{
    /// <summary>
    ///     Determines whether the specified <see cref="ExpandoObject"/> has a given property.
    /// </summary>
    /// <param name="obj">The dynamically assignable object.</param>
    /// <param name="propertyName">Name of the property.</param>
    /// <returns><c>true</c> if the specified <see cref="ExpandoObject"/> contains the given property; otherwise, <c>false</c>.</returns>
    public static bool HasProperty(this ExpandoObject obj, string propertyName)  
    {
        return (obj as IDictionary<string, object>)?.ContainsKey(propertyName) ?? false;
    }
}