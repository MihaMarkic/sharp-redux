using Sharp.Redux.Visualizer.States;
using System.Threading;
using System.Threading.Tasks;

namespace Sharp.Redux.Visualizer.Reducers
{
    public class RootReducer : IReduxReducer<RootState>
    {
        readonly VisualizerReducer visualizerReducer = new VisualizerReducer();
        readonly HubReducer hubReducer = new HubReducer();
        public async Task<RootState> ReduceAsync(RootState state, ReduxAction action, CancellationToken ct)
        {
            var visualizerTask = visualizerReducer.ReduceAsync(state.Visualizer, action, ct);
            var hubTask = hubReducer.ReduceAsync(state.Hub, action, ct);
            await Task.WhenAll(visualizerTask, hubTask);
            return state.Clone(visualizerTask.Result, hubTask.Result);
        }
    }
}
