namespace Sharp.Redux
{
    /// <summary>
    /// Provides link between state and ViewModel types. This interface is usually implemented by ViewModel types for easier update
    /// when linked state changes.
    /// </summary>
    /// <typeparam name="TState">Type of the state.</typeparam>
    public interface IBoundViewModel<TState>
    {
        TState State { get; }
        void Update(TState state);
    }
}
