using System;
using System.Threading.Tasks;

namespace Sharp.Redux
{
    public interface IReduxDispatcher<TState>: IDisposable
    {
        TState State { get; }
        event EventHandler StateChanged;
        void Dispatch(ReduxAction action);
        void Start();
        bool IsProcessorRunning { get; }
        Task StopAsync();
    }
}
