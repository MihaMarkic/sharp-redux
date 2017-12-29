namespace Sharp.Redux
{
    /// <summary>
    /// Interface for declaring that type has a key.
    /// </summary>
    /// <typeparam name="T">Type of the key.</typeparam>
    public interface IKeyedItem<T>: IKeyedItem
    {
        T Key { get; }
    }
    /// <summary>
    /// Interface for declaring that type has a key.
    /// </summary>
    public interface IKeyedItem
    {}
}
