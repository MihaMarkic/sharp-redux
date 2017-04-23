using Sharp.Redux.Playground.Engine.States;
using System.Threading.Tasks;

namespace Sharp.Redux.Playground.Engine
{
    public class PlaygroundReduxDispatcher: ReduxDispatcher<RootState, IReduxReducer<RootState>>, IPlaygroundReduxDispatcher
    {
        public PlaygroundReduxDispatcher(RootState initialState, IReduxReducer<RootState> reducer) :
            base(initialState, reducer, TaskScheduler.FromCurrentSynchronizationContext())
        {

        }
    }
}
