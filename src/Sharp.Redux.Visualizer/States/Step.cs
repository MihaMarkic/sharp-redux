using Righthand.Immutable;
using Sharp.Redux.Visualizer.Core;
using Sharp.Redux.Visualizer.Models;

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
        public DifferenceItem DifferenceItem { get; }
        public bool DifferenceCalculated { get; }

        public Step(int key, ReduxAction action, object state, ObjectData actionData, ObjectTreeItem actionTreeItem, ObjectData stateData, ObjectTreeItem stateTreeItem,
            DifferenceItem differenceItem, bool differenceCalculated)
        {
            Key = key;
            Action = action;
            State = state;
            ActionData = actionData;
            ActionTreeItem = actionTreeItem;
            StateData = stateData;
            StateTreeItem = stateTreeItem;
            DifferenceItem = differenceItem;
            DifferenceCalculated = differenceCalculated;
        }

        public Step Clone(Param<int>? key = null, Param<ReduxAction>? action = null, Param<object>? state = null, Param<ObjectData>? actionData = null, Param<ObjectTreeItem>? actionTreeItem = null, Param<ObjectData>? stateData = null, Param<ObjectTreeItem>? stateTreeItem = null, Param<DifferenceItem>? differenceItem = null, Param<bool>? differenceCalculated = null)
        {
            return new Step(key.HasValue ? key.Value.Value : Key,
action.HasValue ? action.Value.Value : Action,
state.HasValue ? state.Value.Value : State,
actionData.HasValue ? actionData.Value.Value : ActionData,
actionTreeItem.HasValue ? actionTreeItem.Value.Value : ActionTreeItem,
stateData.HasValue ? stateData.Value.Value : StateData,
stateTreeItem.HasValue ? stateTreeItem.Value.Value : StateTreeItem,
differenceItem.HasValue ? differenceItem.Value.Value : DifferenceItem,
differenceCalculated.HasValue ? differenceCalculated.Value.Value : DifferenceCalculated);
        }

        public bool IsKeyEqualTo(IKeyedItem other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }
            Step otherStep = other as Step;
            if (ReferenceEquals(otherStep, null))
            {
                return false;
            }
            return otherStep.Key == Key;
        }
    }
}
