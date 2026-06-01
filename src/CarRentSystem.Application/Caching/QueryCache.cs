namespace CarRentSystem.Application.Caching;

/// <summary>
/// Small generic in-memory cache for deterministic query results.
/// </summary>
public sealed class QueryCache<TKey, TValue>
    where TKey : notnull
{
    private readonly Dictionary<TKey, TValue> _items = new();

    public int Count => _items.Count;

    public TValue GetOrAdd(TKey key, Func<TValue> factory)
    {
        ArgumentNullException.ThrowIfNull(factory);

        if (_items.TryGetValue(key, out var cached))
            return cached;

        var value = factory();
        _items[key] = value;

        return value;
    }

    public void Clear()
    {
        _items.Clear();
    }
}
