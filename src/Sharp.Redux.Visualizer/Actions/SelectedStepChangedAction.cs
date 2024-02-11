namespace Sharp.Redux.Visualizer.Actions
{
    public class SelectedStepChangedAction: ReduxAction
    {
        public int? Key { get; }

        public SelectedStepChangedAction(int? key)
        {
            Key = key;
        }
    }
}
