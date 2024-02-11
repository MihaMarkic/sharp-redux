using System.Collections.Immutable;

namespace Sharp.Redux;

/// <summary>
/// Represents RepliedActions event argument.
/// </summary>
public class RepliedActionsEventArgs: EventArgs
{
    /// <summary>
    /// A list of replied actions.
    /// </summary>
    public ImmutableArray<RepliedAction> Actions { get; }
    /// <summary>
    /// Initializes a new instance of <see cref="RepliedActionsEventArgs"/> that contains <paramref name="actions"/> list of actions.
    /// </summary>
    /// <param name="actions"></param>
    public RepliedActionsEventArgs(ImmutableArray<RepliedAction> actions)
    {
        Actions = actions;
    }
}
/// <summary>
/// Represents information about replied action and its matching state.
/// </summary>
public class RepliedAction
{
    /// <summary>
    /// Action.
    /// </summary>
    public  ReduxAction Action { get; }
    /// <summary>
    /// State after <see cref="Action"/> has been replayed.
    /// </summary>
    public object State { get; }
    public RepliedAction(ReduxAction action, object state)
    {
        Action = action;
        State = state;
    }
}
