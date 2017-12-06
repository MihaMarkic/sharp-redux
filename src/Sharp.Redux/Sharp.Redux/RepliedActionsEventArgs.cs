using System;

namespace Sharp.Redux
{
    public class RepliedActionsEventArgs: EventArgs
    {
        public RepliedAction[] Actions { get; }
        public RepliedActionsEventArgs(RepliedAction[] actions)
        {
            Actions = actions;
        }
    }
    public class RepliedAction
    {
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
}
