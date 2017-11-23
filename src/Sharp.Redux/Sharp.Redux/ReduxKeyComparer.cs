namespace Sharp.Redux
{
    public static class ReduxKeyComparer
    {
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
}
