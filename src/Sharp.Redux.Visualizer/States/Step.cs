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
        public ObjectData StateData { get; }
        public ObjectTreeItem TreeItem { get; }

        public Step(int key, ReduxAction action, object state, ObjectData actionData, ObjectData stateData, ObjectTreeItem treeItem)
        {
            Key = key;
            Action = action;
            State = state;
            ActionData = actionData;
            StateData = stateData;
            TreeItem = treeItem;
        }

        public Step Clone(Param<int>? key = null, Param<ReduxAction>? action = null, Param<object>? state = null, Param<ObjectData>? actionData = null, Param<ObjectData>? stateData = null, Param<ObjectTreeItem>? treeItem = null)
        {
            return new Step(key.HasValue ? key.Value.Value : Key,
action.HasValue ? action.Value.Value : Action,
state.HasValue ? state.Value.Value : State,
actionData.HasValue ? actionData.Value.Value : ActionData,
stateData.HasValue ? stateData.Value.Value : StateData,
treeItem.HasValue ? treeItem.Value.Value : TreeItem);
        }
    }
}
