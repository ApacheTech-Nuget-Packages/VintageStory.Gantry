using System.Collections.Concurrent;

namespace Gantry.Core.Helpers;

/// <summary>
///     A monad that holds a value of type <typeparamref name="T"/> for each mod, isolated by mod ID.
/// </summary>
/// <typeparam name="T"></typeparam>
public class Isolated<T> : IDisposable
{
    private readonly ConcurrentDictionary<string, T> _values = new();

    /// <summary>
    ///     Gets the value for the specified mod ID.
    /// </summary>
    public T Get(string modId) =>
        _values.TryGetValue(modId, out var value)
            ? value
            : throw new InvalidOperationException($"No value of type {typeof(T).FullName} has been set for mod '{modId}'.");

    /// <summary>
    ///     Determines if a value has been set for the specified mod ID.
    /// </summary>
    public bool HasValue(string modId) => _values.ContainsKey(modId);

    /// <summary>
    ///     Sets the value for the specified mod ID.
    /// </summary>
    public void Set(string modId, T value)
    {
        _values[modId] = value;
    }

    /// <summary>
    ///     Removes the value for the specified mod ID.
    /// </summary>
    public void Dispose(string modId)
    {
        _values.TryRemove(modId, out _);
    }

    /// <summary>
    ///     Removes all values for all mod IDs.
    /// </summary>
    public void Dispose()
    {
        _values.Clear();
    }

    /// <summary>
    ///     Returns all mod IDs currently registered.
    /// </summary>
    public IEnumerable<string> GetAllModIds() => _values.Keys;
}
