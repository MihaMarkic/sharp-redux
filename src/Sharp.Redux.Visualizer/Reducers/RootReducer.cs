using Sharp.Redux.Visualizer.Actions;
using Sharp.Redux.Visualizer.States;
using System.Threading;
using System.Threading.Tasks;

namespace Sharp.Redux.Visualizer.Reducers
{
    public class RootReducer : IReduxReducer<RootState>
    {
        public Task<RootState> ReduceAsync(RootState state, ReduxAction action, CancellationToken ct)
        {
            RootState result;
            switch (action)
            {
                case InsertNewAction insertNew:
                    int key = state.Steps.Length;
                    result = state.Clone(steps: state.Steps.Spread(new Step(key, insertNew.Action, insertNew.State)));
                    break;
                default:
                    result = state;
                    break;
            }
            return Task.FromResult(result);
        }
    }
}
