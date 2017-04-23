using Sharp.Redux.Playground.Engine.States;
using System.Threading;
using System.Threading.Tasks;

namespace Sharp.Redux.Playground.Engine.Reducers
{
    public class RootReducer : IReduxReducer<RootState>
    {
        public async Task<RootState> ReduceAsync(RootState state, ReduxAction action, CancellationToken ct)
        {
            var navigationReduceTask = Task.Run(() => NavigationReducer.Reduce(state.Navigation, action));
            var newFirstPageReduceTask = Task.Run(() => FirstPageReducer.Reduce(state.FirstPage, action));

            return state.Clone(
                navigation: await navigationReduceTask,
                firstPage: await newFirstPageReduceTask
            );
        }
    }
}
