using System.Collections.Immutable;

namespace Sharp.Redux;

/// <summary>
/// Represents StateChanged event argument with a non generic state instance type.
/// </summary>
public class StateChangedEventArgs: EventArgs
{
    /// <summary>
    /// Action that triggered this state change.
    /// </summary>
    public readonly ReduxAction Action;
    public readonly object State;
    /// <summary>
    /// Client can signal that it is processing asynchronous actions and that dispatcher shall wait before processing next action.
    /// </summary>
    public ImmutableArray<Task> RunningTasks { get; private set; } = ImmutableArray<Task>.Empty;
    /// <summary>
    /// Initializes a new instance of <see cref="StateChangedEventArgs"/> containing action that triggered change.
    /// </summary>
    /// <param name="action">Action that triggered this state change.</param>
    /// /// <param name="state">The state after the action.</param>
    public StateChangedEventArgs(ReduxAction action, object state)
    {
        Action = action;
        State = state;
    }
    /// <summary>
    /// Adds a new task to the list of tasks to wait for. Dispatcher will wait for this task to end before processing next action.
    /// </summary>
    /// <param name="task">An instance of task to await.</param>
    public void AddRunningTask(Task task)
    {
        if (RunningTasks == null)
        {
            RunningTasks = [ task ];
        }
        else
        {
            RunningTasks = RunningTasks.Add(task);
        }
    }
}
/// <summary>
/// Represents StateChanged event argument.
/// </summary>
public class StateChangedEventArgs<TState> : StateChangedEventArgs
    where TState: class
{
    /// <summary>
    /// Initializes a new instance of <see cref="StateChangedEventArgs"/> containing action that triggered change.
    /// </summary>
    /// <param name="action">Action that triggered this state change.</param>
    /// <param name="state">The state after the action.</param>
    public StateChangedEventArgs(ReduxAction action, TState state) : base(action, state)
    { }
    public new TState State => (TState)base.State;
}
