using Sharp.Redux.Visualizer.Actions;
using Sharp.Redux.Visualizer.Services.Implementation;
using Sharp.Redux.Visualizer.States;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sharp.Redux.Visualizer.Reducers
{
    public class RootReducer : IReduxReducer<RootState>
    {
        public async Task<RootState> ReduceAsync(RootState state, ReduxAction action, CancellationToken ct)
        {
            RootState result = state;
            switch (action)
            {
                case InsertNewAction insertNew:
                    int key = state.Steps.Length;
                    var actionDataTask = PropertiesCollector.CollectAsync(insertNew.Action, ct);
                    var stateDataTask = PropertiesCollector.CollectAsync(insertNew.State, ct);
                    await Task.WhenAll(actionDataTask, stateDataTask).ConfigureAwait(false);
                    result = state.Clone(steps: state.Steps.Spread(
                        new Step(key, insertNew.Action, insertNew.State, actionData: actionDataTask.Result, stateData: stateDataTask.Result, treeItem: null)));
                    break;
                case GenerateTreeHierarchyAction generateTreeHierarchy:
                    {
                        Step selectedStep = state.SelectedStep;
                        if (selectedStep?.TreeItem == null)
                        {
                            var hierachy = StateFormatter.ToTreeHierarchy(selectedStep.StateData);
                            Step updated = selectedStep.Clone(treeItem: hierachy);
                            result = state.Clone(steps: state.Steps.Replace(selectedStep, updated), selectedStep: updated);
                        }
                    }
                    break;
                case SelectedStepChangedAction selectedStepChanged:
                    {
                        var selectedStep = selectedStepChanged.Key.HasValue ? state.Steps.Single(s => s.Key == selectedStepChanged.Key) : null;
                        result = state.Clone(selectedStep: selectedStep);
                    }
                    break;
            }
            return result;
        }
    }
}
