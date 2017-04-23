using Righthand.Immutable;

namespace Sharp.Redux.Visualizer.States
{
    public class Step: IKeyedItem<int>
    {
        public int Key { get; }
        public ReduxAction Action { get; }
        public object State { get; }

        public Step(int key, ReduxAction action, object state)
        {
            Key = key;
            Action = action;
            State = state;
        }

        public Step Clone(Param<int>? key = null, Param<ReduxAction>? action = null, Param<object>? state = null)
        {
            return new Step(key.HasValue ? key.Value.Value : Key,
action.HasValue ? action.Value.Value : Action,
state.HasValue ? state.Value.Value : State);
        }
    }
}
