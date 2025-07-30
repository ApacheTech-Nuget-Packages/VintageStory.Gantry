namespace Gantry.Services.EasyX.Extensions;

/// <summary>
///     Provides extension methods for working with enumerables.
/// </summary>
public static class EnumerableExtensions
{
    /// <summary>
    ///     Determines whether the specified item is one of the options provided.
    /// </summary>
    /// <returns>Returns true if the item is one of the options; otherwise, false.</returns>
    public static bool IsOneOf<T>(this T item, params T[] options) => options.Contains(item);
}