using System;
using System.Threading.Tasks;

namespace Sharp.Redux
{
    public interface IReduxDispatcher<TState>: IReduxDispatcher, IDisposable
    {
        new TState State { get; }
        void Dispatch(ReduxAction action);
        void Start();
        bool IsProcessorRunning { get; }
        Task StopAsync();
    }

    public interface IReduxDispatcher
    {
        event EventHandler<StateChangedEventArgs> StateChanged;
        object State { get; }
    }
}
