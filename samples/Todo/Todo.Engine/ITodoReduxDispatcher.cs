using Sharp.Redux;
using Todo.Engine.States;

namespace Todo.Engine
{
    public interface ITodoReduxDispatcher : IReduxDispatcher<RootState>
    { }
}
