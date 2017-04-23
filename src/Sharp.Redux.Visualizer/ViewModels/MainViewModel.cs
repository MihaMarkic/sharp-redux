using Sharp.Redux.Visualizer.Actions;
using Sharp.Redux.Visualizer.Core;
using Sharp.Redux.Visualizer.States;
using System.Collections.ObjectModel;

namespace Sharp.Redux.Visualizer.ViewModels
{
    public class MainViewModel : NotificableObject
    {
        private readonly IReduxDispatcher sourceDispatcher;
        public ObservableCollection<StepViewModel> Steps { get; } = new ObservableCollection<StepViewModel>();
        public int IgnoredNamespaceElementsCount { get; set; }
        public MainViewModel(IReduxDispatcher sourceDispatcher)
        {
            this.sourceDispatcher = sourceDispatcher;
            sourceDispatcher.StateChanged += SourceDispatcher_StateChanged;
            VisualizerDispatcher.Default.StateChanged += Default_StateChanged;
        }

        private void Default_StateChanged(object sender, StateChangedEventArgs e)
        {
            ReduxMerger.Merge<int, Step, StepViewModel>(VisualizerDispatcher.Default.State.Steps, Steps, step => new StepViewModel(step));
        }

        private void SourceDispatcher_StateChanged(object sender, StateChangedEventArgs e)
        {
            VisualizerDispatcher.Default.Dispatch(new InsertNewAction(e.Action, sourceDispatcher.State));
        }
    }
}
