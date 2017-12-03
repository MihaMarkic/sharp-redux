using Sharp.Redux.Visualizer.Actions;
using Sharp.Redux.Visualizer.Core;
using Sharp.Redux.Visualizer.States;
using System;
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
            dispatcher.StateChanged += Default_StateChanged;
            GotoStepCommand = new RelayCommand(GotoStep, () => selectedStep != null && !IsResetingState);
        }

        public async void GotoStep()
        {
            IsResetingState = true;
            try
            {
                await sourceDispatcher.ResetToInitialStateAsync();
                var actions = dispatcher.State.Steps
                    .TakeWhile(s => !Equals(s, selectedStep.State)).Union(new[] { selectedStep.State })
                    .Select(s => s.Action)
                    .ToArray();
                ResetingActionsCount = actions.Length;
                ResetingActionCurrent = 0;
                await sourceDispatcher.ReplayActionsAsync(actions, new UISyncedProgress<int>(p => ResetingActionCurrent = p), ct: default);
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
