using Righthand.Immutable;
using Sharp.Redux.Visualizer.Core;

namespace Sharp.Redux.Visualizer.States
{
    public class Step: IKeyedItem<int>
    {
        public int Key { get; }
        public ReduxAction Action { get; }
        public object State { get; }
        public ObjectData ActionData { get; }
        public ObjectTreeItem ActionTreeItem { get; }
        public ObjectData StateData { get; }
        public ObjectTreeItem StateTreeItem { get; }

        public Step(int key, ReduxAction action, object state, ObjectData actionData, ObjectTreeItem actionTreeItem, ObjectData stateData, ObjectTreeItem stateTreeItem)
        {
            Key = key;
            Action = action;
            State = state;
            ActionData = actionData;
            ActionTreeItem = actionTreeItem;
            StateData = stateData;
            StateTreeItem = stateTreeItem;
        }

        public Step Clone(Param<int>? key = null, Param<ReduxAction>? action = null, Param<object>? state = null, Param<ObjectData>? actionData = null, Param<ObjectTreeItem>? actionTreeItem = null, Param<ObjectData>? stateData = null, Param<ObjectTreeItem>? stateTreeItem = null)
        {
            return new Step(key.HasValue ? key.Value.Value : Key,
action.HasValue ? action.Value.Value : Action,
state.HasValue ? state.Value.Value : State,
actionData.HasValue ? actionData.Value.Value : ActionData,
actionTreeItem.HasValue ? actionTreeItem.Value.Value : ActionTreeItem,
stateData.HasValue ? stateData.Value.Value : StateData,
stateTreeItem.HasValue ? stateTreeItem.Value.Value : StateTreeItem);
        }
    }
}
