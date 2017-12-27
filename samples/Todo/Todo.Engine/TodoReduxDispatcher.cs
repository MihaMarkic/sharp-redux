using Todo.Engine.States;
using System.Threading.Tasks;
using Sharp.Redux;

namespace Todo.Engine
{
    public class TodoReduxDispatcher: ReduxDispatcher<RootState, IReduxReducer<RootState>>, ITodoReduxDispatcher
    {
        public TodoReduxDispatcher(RootState initialState, IReduxReducer<RootState> reducer) :
            base(initialState, reducer, TaskScheduler.FromCurrentSynchronizationContext())
        {

        }
    }
}
