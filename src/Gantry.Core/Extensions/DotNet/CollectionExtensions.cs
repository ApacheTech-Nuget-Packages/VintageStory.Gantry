using JetBrains.Annotations;
using System;
using System.Collections.Generic;

namespace Gantry.Core.Extensions.DotNet
{
    /// <summary>
    ///     Extension methods to aid when working with collections.
    /// </summary>
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public static class CollectionExtensions
    {
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
        ///     Filters a list, but preserves the original indices, within a sorted dictionary.
        /// </summary>
        /// <typeparam name="T">The type of the items held in the collection.</typeparam>
        /// <param name="collection">The collection to filter.</param>
        /// <param name="predicate">A function used to filter the collection.</param>
        public static SortedDictionary<int, T> ToFilteredSortedDictionary<T>(this IList<T> collection, Func<T, bool> predicate)
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
    }
}