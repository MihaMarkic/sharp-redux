using Righthand.Immutable;

namespace Sharp.Redux.Actions.Visualizer
{
    public class InsertNewAction: ReduxAction
    {
        public ReduxAction Action { get; }
        public object State { get; }

        public InsertNewAction(ReduxAction action, object state)
        {
            Action = action;
            State = state;
        }

        public InsertNewAction Clone(Param<ReduxAction>? action = null, Param<object>? state = null)
        {
            return new InsertNewAction(action.HasValue ? action.Value.Value : Action,
state.HasValue ? state.Value.Value : State);
        }
    }
}
