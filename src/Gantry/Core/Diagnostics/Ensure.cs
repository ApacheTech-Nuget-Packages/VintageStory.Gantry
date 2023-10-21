namespace Gantry.Core.Diagnostics;

/// <summary>
///     Helper methods to ensure specific criteria.
/// </summary>
public static class Ensure
{
    /// <summary>
    ///     Ensures that a member is populated with a specific value. If the hash code is different to the current value, the value is changed.
    /// </summary>
    /// <typeparam name="T">The type of value to work with.</typeparam>
    /// <param name="source">The source object.</param>
    /// <param name="target">The target object.</param>
    /// <returns></returns>
    public static T PopulatedWith<T>(T source, T target)
    {
        if (source is not null && source.GetHashCode().Equals(target.GetHashCode())) return source;
        return target;
    }
}