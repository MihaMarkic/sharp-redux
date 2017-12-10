namespace Sharp.Redux
{
    public interface IBoundViewModel<TState>
    {
        TState State { get; }
        void Update(TState state);
    }
}
