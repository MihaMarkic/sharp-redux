using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sharp.Redux
{
    public interface IReduxDispatcher<TState>: IReduxDispatcher, IDisposable
    {
        new TState State { get; }
        Task ResetStateAsync(TState newState);
    }

    public interface IReduxDispatcher
    {
        event EventHandler<StateChangedEventArgs> StateChanged;
        event EventHandler<RepliedActionsEventArgs> RepliedActions;
        void Dispatch(ReduxAction action);
        void Start();
        bool IsProcessorRunning { get; }
        Task StopAsync();
        object State { get; }
        Task ResetStateAsync(object newState);
        Task ResetToInitialStateAsync();
        Task ReplayActionsAsync(ReduxAction[] actions, IProgress<int> progress, CancellationToken ct);
    }
}
