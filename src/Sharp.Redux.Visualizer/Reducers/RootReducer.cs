using Sharp.Redux.Visualizer.Actions;
using Sharp.Redux.Visualizer.Models;
using Sharp.Redux.Visualizer.Services.Implementation;
using Sharp.Redux.Visualizer.States;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sharp.Redux.Visualizer.Reducers
{
    public class RootReducer : IReduxReducer<RootState>
    {
        public async Task<RootState> ReduceAsync(RootState state, ReduxAction action, CancellationToken ct)
        {
            RootState result;
            switch (action)
            {
                case InsertNewAction insertNew:
                    int key = state.Steps.Length;
                    var actionDataTask = Task.Run(() =>PropertiesCollector.Collect(insertNew.Action), ct);
                    var stateDataTask = Task.Run(() => PropertiesCollector.Collect(insertNew.State), ct);
                    await actionDataTask.ConfigureAwait(false);
                    var actionTreeItem = await Task.Run(() => {
                        string actionName = StateFormatter.GetActionName(insertNew.Action);
                        return StateFormatter.ToTreeHierarchy(actionDataTask.Result, actionName);
                    }).ConfigureAwait(false);
                    await stateDataTask.ConfigureAwait(false);
                    
                    result = state.Clone(steps: state.Steps.Spread(
                        new Step(key, insertNew.Action, insertNew.State, actionData: actionDataTask.Result, 
                            actionTreeItem: actionTreeItem,
                            stateData: stateDataTask.Result, stateTreeItem: null, differenceItem: null, differenceCalculated: false)));
                    break;
                case GenerateTreeHierarchyAction generateTreeHierarchy:
                    {
                        Step selectedStep = state.SelectedStep;
                        result = state;
                        if (selectedStep != null)
                        {
                            if (selectedStep.StateTreeItem is null)
                            {
                                var hierarchy = StateFormatter.ToTreeHierarchy(selectedStep.StateData);
                                Step updated = selectedStep.Clone(stateTreeItem: hierarchy);
                                result = result.Clone(steps: state.Steps.Replace(selectedStep, updated), selectedStep: updated);
                                selectedStep = updated;
                            }
                            if (!selectedStep.DifferenceCalculated)
                            {
                                // first check if previous step has StateTree
                                var selectedStepIndex = Array.IndexOf(result.Steps, selectedStep);
                                var previousStep = selectedStepIndex > 0 ? result.Steps[selectedStepIndex - 1] : null;
                                ObjectTreeItem previousHierarchy = null;
                                if (previousStep != null)
                                {
                                    if (previousStep.StateTreeItem == null)
                                    {
                                        previousHierarchy = StateFormatter.ToTreeHierarchy(previousStep.StateData);
                                        Step previousUpdated = previousStep.Clone(stateTreeItem: previousHierarchy);
                                        result = result.Clone(steps: result.Steps.Replace(previousStep, previousUpdated));
                                    }
                                    else
                                    {
                                        previousHierarchy = previousStep.StateTreeItem;
                                    }
                                }
                                var difference = TreeComparer.CreateDifferenceTree(previousHierarchy, selectedStep.StateTreeItem);
                                Step updated = selectedStep.Clone(differenceItem: difference, differenceCalculated: true);
                                result = result.Clone(steps: result.Steps.Replace(selectedStep, updated), selectedStep: updated);
                            }
                        }
                    }
                    break;
                case SelectedStepChangedAction selectedStepChanged:
                    {
                        var selectedStep = selectedStepChanged.Key.HasValue ? state.Steps.Single(s => s.Key == selectedStepChanged.Key) : null;
                        result = state.Clone(selectedStep: selectedStep);
                    }
                    break;
                default:
                    result = state;
                    break;
            }
            return result;
        }
    }
}
