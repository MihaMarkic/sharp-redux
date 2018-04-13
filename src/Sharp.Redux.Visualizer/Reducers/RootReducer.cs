using Sharp.Redux.Visualizer.States;
using System.Threading;
using System.Threading.Tasks;

namespace Sharp.Redux.Visualizer.Reducers
{
    public class RootReducer : IReduxReducer<RootState>
    {
        readonly VisualizerReducer visualizerReducer = new VisualizerReducer();
        public async Task<RootState> ReduceAsync(RootState state, ReduxAction action, CancellationToken ct)
        {
            var visualizerState = await visualizerReducer.ReduceAsync(state.Visualizer, action, ct);
            return state.Clone(visualizerState);
        }
    }
}
