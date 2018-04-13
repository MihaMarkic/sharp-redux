using Sharp.Redux.Visualizer.Actions.Hub;
using Sharp.Redux.Visualizer.States;
using System.Threading;
using System.Threading.Tasks;

namespace Sharp.Redux.Visualizer.Reducers
{
    public class HubReducer : IReduxReducer<HubState>
    {
        public Task<HubState> ReduceAsync(HubState state, ReduxAction action, CancellationToken ct)
        {
            switch (action)
            {
                case FetchSessionsAction fetchSession:
                    return Task.FromResult(state.Clone(fetchSession.Sessions));
                default:
                    return Task.FromResult(state);
            }
        }
    }
}
