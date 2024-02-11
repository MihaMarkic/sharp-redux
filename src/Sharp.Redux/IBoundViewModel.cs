namespace Sharp.Redux;

/// <summary>
/// Provides link between state and ViewModel types. This interface is usually implemented by ViewModel types for easier update
/// when linked state changes.
/// </summary>
/// <typeparam name="TState">Type of the state.</typeparam>
public interface IBoundViewModel<TState>
{
    /// <summary>
    /// Linked state.
    /// </summary>
    TState State { get; }
    /// <summary>
    /// Updates target type given the new state.
    /// </summary>
    /// <param name="state"></param>
    void Update(TState state);
}
