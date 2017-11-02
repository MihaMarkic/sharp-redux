namespace Sharp.Redux
{
    public interface IKeyedItem<T>: IKeyedItem
    {
        T Key { get; }
    }

    public  interface IKeyedItem
    {
        bool IsKeyEqualTo(IKeyedItem other);
    }
}
