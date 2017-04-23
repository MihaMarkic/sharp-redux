namespace Sharp.Redux.Visualizer.ViewModels
{
    public class StepViewModel
    {
        public ReduxAction Action { get; }
        public object State { get; }
        public StepViewModel(ReduxAction action, object state)
        {
            Action = action;
            State = state;
        }

        public string ActionName => Action.GetType().Name;
    }
}
