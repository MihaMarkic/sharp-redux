using Newtonsoft.Json;
using Sharp.Redux.Visualizer.States;

namespace Sharp.Redux.Visualizer.ViewModels
{
    public class StepViewModel: IKeyedItem<int>
    {
        public Step Step { get; }
        public string ActionName { get; }
        public StepViewModel(Step step)
        {
            Step = step;
            ActionName = GetActionName();
        }
        public int Key => Step.Key;

        public string GetActionName()
        {
            var fullName = Step.Action.GetType().FullName;
            foreach (string prefix in ReduxVisualizer.IgnoredNamespacePrefixes)
            {
                if (fullName.StartsWith(prefix, System.StringComparison.Ordinal))
                {
                    return fullName.Substring(prefix.Length + 1);
                }
            }
            return fullName;
        }
        public string ActionContent => JsonConvert.SerializeObject(Step.Action);

        public string StateJson => JsonConvert.SerializeObject(Step.State);
    }
}
