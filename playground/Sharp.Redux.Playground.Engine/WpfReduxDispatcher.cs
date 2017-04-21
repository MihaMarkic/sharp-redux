using Sharp.Redux.Playground.Engine.State;
using System.Threading.Tasks;

namespace Sharp.Redux.Playground.Engine
{
    public class WpfReduxDispatcher: ReduxDispatcher<RootState, IReduxReducer<RootState>>, IPlaygroundReduxDispatcher
    {
        public WpfReduxDispatcher(RootState initialState, IReduxReducer<RootState> reducer) :
            base(initialState, reducer, TaskScheduler.FromCurrentSynchronizationContext())
        {

        }
    }
}
