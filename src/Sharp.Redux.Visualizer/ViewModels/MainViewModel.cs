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
        readonly IReduxDispatcher sourceDispatcher;
        readonly VisualizerDispatcher dispatcher = VisualizerDispatcher.Default;
        public ObservableCollection<StepViewModel> Steps { get; } = new ObservableCollection<StepViewModel>();
        StepViewModel selectedStep;
        public  RelayCommand GotoStepCommand { get; }
        public bool IsResetingState { get; private set; }
        public int ResetingActionsCount { get; private set; }
        public int ResetingActionCurrent { get; private set; }
        public MainViewModel(IReduxDispatcher sourceDispatcher)
        {
            this.sourceDispatcher = sourceDispatcher;
            sourceDispatcher.StateChanged += SourceDispatcher_StateChanged;
            sourceDispatcher.RepliedActions += SourceDispatcher_RepliedActions;
            dispatcher.StateChanged += Default_StateChanged;
            GotoStepCommand = new RelayCommand(GotoStep, () => selectedStep != null && !IsResetingState);
        }
        public async void GotoStep()
        {
            IsResetingState = true;
            try
            {
                await sourceDispatcher.ResetToInitialStateAsync();
                var actions = dispatcher.InitialState.Steps
                    .TakeWhile(s => !Equals(s, selectedStep.State)).Union(new[] { selectedStep.State })
                    .Select(s => s.Action)
                    .ToArray();
                ResetingActionsCount = actions.Length;
                ResetingActionCurrent = 0;
                await sourceDispatcher.ReplayActionsAsync(actions, new UISyncedProgress<int>(p => ResetingActionCurrent = p), ct: default);
                sourceDispatcher.Start();
            }
            finally
            {
                IsResetingState = false;
            }
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
                    GotoStepCommand.RaiseCanExecuteChanged();
                    break;
                case nameof(IsResetingState):
                    GotoStepCommand.RaiseCanExecuteChanged();
                    break;
            }
            base.OnPropertyChanged(name);
        }

        private void Default_StateChanged(object sender, StateChangedEventArgs<RootState> e)
        {
            RootState state = e.State;
            ReduxMerger.MergeList<int, Step, StepViewModel>(state.Steps, Steps, step => new StepViewModel(step));
            SelectedStep = state.SelectedStep != null ? Steps.Single(s => s.Key == state.SelectedStep.Key): null;
        }

        private void SourceDispatcher_StateChanged(object sender, StateChangedEventArgs e)
        {
            // action can be null when replay dispatches StateChanged event at the end
            if (e.Action != null)
            {
                dispatcher.Dispatch(new InsertNewAction(e.Action, e.State));
            }
        }
        private void SourceDispatcher_RepliedActions(object sender, RepliedActionsEventArgs e)
        {
            foreach (var pair in e.Actions)
            {
                dispatcher.Dispatch(new InsertNewAction(pair.Action, pair.State));
            }
        }
    }
}
