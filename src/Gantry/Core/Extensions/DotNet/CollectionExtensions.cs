namespace Gantry.Core.Extensions.DotNet;

/// <summary>
///     Extension methods to aid when working with collections.
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class CollectionExtensions
{
    /// <summary>
    ///     Determines whether the specified index is outside the bounds of the given collection.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <param name="value">The index to check.</param>
    /// <param name="collection">The collection to check against.</param>
    /// <returns><c>true</c> if the index is outside the bounds of the collection; otherwise, <c>false</c>.</returns>
    public static bool IsOutsideBounds<T>(this int value, IEnumerable<T> collection)
    {
        if (value < 0 || !collection.TryGetNonEnumeratedCount(out var count)) return true;
        return value >= count;
    }
    
    /// <summary>
    ///     Retrieves a random element from the collection.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <param name="collection">The collection from which to retrieve a random element.</param>
    /// <returns>A random element from the collection.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the collection is null or empty.</exception>
    public static T GetRandomValue<T>(this ICollection<T> collection)
    {
        if (collection is null || collection.Count == 0)
            throw new InvalidOperationException("Cannot retrieve a random value from an empty or null collection.");
        var index = Random.Shared.Next(collection.Count);
        foreach (var item in collection) if (index-- == 0) return item;
        throw new InvalidOperationException("Unexpected error occurred while retrieving a random value.");
    }

    /// <summary>
    ///     Preserves the original indices of a list, within a sorted dictionary.
    /// </summary>
    /// <typeparam name="T">The type of the items held in the collection.</typeparam>
    /// <param name="collection">The collection to sort.</param>
    public static SortedDictionary<int, T> ToSortedDictionary<T>(this IList<T> collection)
    {
        var list = new SortedDictionary<int, T>();
        for (var i = 0; i < collection.Count; i++)
        {
            list.Add(i, collection[i]);
        }
        return list;
    }

    /// <summary>
    ///     Tries to invoke the specified action on the first element of the collection, if it exists.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <param name="collection">The collection to operate on.</param>
    /// <param name="action">The action to invoke on the first element of the collection.</param>
    /// <returns>Returns <c>true</c> if the action was successfully invoked, otherwise <c>false</c>.</returns>
    public static bool TryInvokeFirst<T>(this IEnumerable<T> collection, Action<T> action)
    {
        var item = collection.FirstOrDefault();
        if (item is null) return false;
        action(item);
        return true;
    }

    /// <summary>
    ///     Invokes the specified action on the first element of the collection that matches the given predicate.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <param name="collection">The collection to operate on.</param>
    /// <param name="predicate">The predicate to filter the elements of the collection.</param>
    /// <param name="action">The action to invoke on an element that matches the predicate.</param>
    public static bool TryInvokeFirst<T>(this IEnumerable<T> collection, System.Func<T, bool> predicate, Action<T> action)
    {
        var item = collection.FirstOrDefault(predicate);
        if (item is null) return false;
        action(item);
        return true;
    }

    /// <summary>
    ///     Invokes the specified action on all elements of the collection that match the given predicate.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <param name="collection">The collection to operate on.</param>
    /// <param name="predicate">The predicate to filter the elements of the collection.</param>
    /// <param name="action">The action to invoke on each element that matches the predicate.</param>
    public static void InvokeWhere<T>(this IEnumerable<T> collection, System.Func<T, bool> predicate, Action<T> action)
    {
        var items = collection.Where(predicate);
        foreach (var item in items) action(item);
    }

    /// <summary>
    ///     Filters a list, but preserves the original indices, within a sorted dictionary.
    /// </summary>
    /// <typeparam name="T">The type of the items held in the collection.</typeparam>
    /// <param name="collection">The collection to filter.</param>
    /// <param name="predicate">A function used to filter the collection.</param>
    public static SortedDictionary<int, T> ToFilteredSortedDictionary<T>(this IList<T> collection, System.Func<T, bool> predicate)
    {
        var comparer = Comparer<int>.Create((a, b) => b.CompareTo(a));
        var list = new SortedDictionary<int, T>(comparer);
        for (var i = 0; i < collection.Count; i++)
        {
            if (!predicate(collection[i])) continue;
            list.Add(i, collection[i]);
        }
        return list;
    }

    /// <summary>
    ///     Determines whether a single item is within an arbitrary array of items of the same type.
    /// </summary>
    /// <typeparam name="T">The type of item.</typeparam>
    /// <param name="item">The items to find.</param>
    /// <param name="options">The array of items to test against.</param>
    public static bool IsOneOf<T>(this T item, params T[] options) => options.Contains(item);

    /// <summary>
    ///     Invokes the specified action for each item in the collection.
    /// </summary>
    /// <param name="collection">The collection of items to invoke the action on.</param>
    /// <param name="action">The action to invoke for each item.</param>
    public static void InvokeForAll<T>(this IEnumerable<T> collection, Action<T> action)
    {
        if (collection is null) return;
        foreach (var item in collection) action(item);
    }

    /// <summary>
    ///     Adds an item to the list associated with the specified key, or creates the list if it does not exist.
    /// </summary>
    /// <typeparam name="TKey">The type of the dictionary key.</typeparam>
    /// <typeparam name="T">The type of the item to add to the list.</typeparam>
    /// <param name="dictionary">The dictionary to modify.</param>
    /// <param name="key">The key of the list to modify or create.</param>
    /// <param name="item">The item to add to the list.</param>
    public static void AddToList<TKey, T>(this IDictionary<TKey, List<T>> dictionary, TKey key, T item) where TKey : notnull
    {
        if (dictionary == null) throw new ArgumentNullException(nameof(dictionary));
        if (!dictionary.TryGetValue(key, out var value)) dictionary[key] = value = [];
        value.Add(item);
    }
}