using System;

namespace Sharp.Redux
{
    public class StateChangedEventArgs: EventArgs
    {
        public readonly ReduxAction Action;
        public StateChangedEventArgs(ReduxAction action)
        {
            Action = action;
        }
    }
}
