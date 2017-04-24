using Sharp.Redux.Visualizer.Core;
using Sharp.Redux.Visualizer.Services.Implementation;
using Sharp.Redux.Visualizer.States;

namespace Sharp.Redux.Visualizer.ViewModels
{
    public class StepViewModel: NotificableObject, IKeyedItem<int>, IBoundViewModel<Step>
    {
        public Step State { get; private set; }
        public string ActionName { get; private set; }
        public StepViewModel(Step step)
        {
            Update(step);
            ActionName = GetActionName();
        }
        public void Update(Step step)
        {
            State = step;
            // only RootStateNode can be updated
            OnPropertyChanged(nameof(RootStateNode));
        }
        public int Key => State.Key;

        public string GetActionName()
        {
            var fullName = State.Action.GetType().FullName;
            foreach (string prefix in ReduxVisualizer.IgnoredNamespacePrefixes)
            {
                if (fullName.StartsWith(prefix, System.StringComparison.Ordinal))
                {
                    return fullName.Substring(prefix.Length + 1);
                }
            }
            return fullName;
        }
        public string ActionContent => PropertiesCollector.DataToString(State.ActionData);

        public ObjectTreeItem[] RootStateNode => new ObjectTreeItem[] { State.TreeItem };
    }
}
