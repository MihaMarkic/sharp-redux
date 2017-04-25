using Sharp.Redux.Visualizer.Core;
using Sharp.Redux.Visualizer.Services.Implementation;
using Sharp.Redux.Visualizer.States;

namespace Sharp.Redux.Visualizer.ViewModels
{
    public class StepViewModel: NotificableObject, IKeyedItem<int>, IBoundViewModel<Step>
    {
        public Step State { get; private set; }
        public StepViewModel(Step step)
        {
            Update(step);
        }
        public void Update(Step step)
        {
            State = step;
            // only RootStateNode can be updated
            OnPropertyChanged(nameof(RootStateNode));
        }
        public int Key => State.Key;
        
        public string ActionContent => PropertiesCollector.DataToString(State.ActionData);

        public ObjectTreeItem[] RootStateNode => new ObjectTreeItem[] { State.StateTreeItem };
        public ObjectTreeItem[] ActionNode => new ObjectTreeItem[] { State.ActionTreeItem };
    }
}
