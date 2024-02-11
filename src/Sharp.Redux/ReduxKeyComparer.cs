namespace Sharp.Redux;

/// <summary>
/// Represents a key comparer.
/// </summary>
public static class ReduxKeyComparer
{
    /// <summary>
    /// Compares two keyed types.
    /// </summary>
    /// <typeparam name="TKey">Type of the key.</typeparam>
    /// <param name="source">Source object.</param>
    /// <param name="other">Object to compare <paramref name="source"/> with.</param>
    /// <returns>True when objects are equal, false otherwise.</returns>
    public static bool AreKeyEqual<TKey>(this IKeyedItem<TKey> source, IKeyedItem other)
    {
        if (ReferenceEquals(source, other))
        {
            return true;
        }
        if (ReferenceEquals(other, null) || ReferenceEquals(source, null) || source.GetType() != other.GetType())
        {
            return false;
        }
        return Equals(source.Key, ((IKeyedItem<TKey>)other).Key);
    }
}
