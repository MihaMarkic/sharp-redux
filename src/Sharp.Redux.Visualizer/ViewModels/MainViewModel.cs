using Sharp.Redux.Visualizer.Actions;
using Sharp.Redux.Visualizer.Core;
using Sharp.Redux.Visualizer.States;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Sharp.Redux.Visualizer.ViewModels
{
    public class MainViewModel : NotificableObject
    {
        private readonly IReduxDispatcher sourceDispatcher;
        private readonly VisualizerDispatcher dispatcher = VisualizerDispatcher.Default;
        public ObservableCollection<StepViewModel> Steps { get; } = new ObservableCollection<StepViewModel>();
        private StepViewModel selectedStep;
        public MainViewModel(IReduxDispatcher sourceDispatcher)
        {
            this.sourceDispatcher = sourceDispatcher;
            sourceDispatcher.StateChanged += SourceDispatcher_StateChanged;
            dispatcher.StateChanged += Default_StateChanged;
        }

        public StepViewModel SelectedStep
        {
            get
            {
                return selectedStep;
            }
            set
            {
                if (selectedStep == value)
                    return;
                selectedStep = value;
                OnPropertyChanged();
            }
        }

        protected override void OnPropertyChanged([CallerMemberName] string name = null)
        {
            switch (name)
            {
                case nameof(SelectedStep):
                    dispatcher.Dispatch(new SelectedStepChangedAction(SelectedStep?.Key));
                    dispatcher.Dispatch(new GenerateTreeHierarchyAction());
                    break;
            }
            base.OnPropertyChanged(name);
        }

        private void Default_StateChanged(object sender, StateChangedEventArgs e)
        {
            RootState state = VisualizerDispatcher.Default.State;
            ReduxMerger.MergeList<int, Step, StepViewModel>(state.Steps, Steps, step => new StepViewModel(step));
            SelectedStep = state.SelectedStep != null ? Steps.Single(s => s.Key == state.SelectedStep.Key): null;
        }

        private void SourceDispatcher_StateChanged(object sender, StateChangedEventArgs e)
        {
            dispatcher.Dispatch(new InsertNewAction(e.Action, sourceDispatcher.State));
        }
    }
}
