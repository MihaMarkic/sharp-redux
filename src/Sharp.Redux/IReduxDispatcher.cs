using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sharp.Redux
{
    public interface IReduxDispatcher<TState>: IReduxDispatcher, IDisposable
    {
        ///// <summary>
        ///// Current state.
        ///// </summary>
        //new TState State { get; }
        /// <summary>
        /// Resets state to given <paramref name="newState"/>.
        /// </summary>
        /// <returns>An instance of <see cref="Task"/>.</returns>
        Task ResetStateAsync(TState newState);
        new event EventHandler<StateChangedEventArgs<TState>> StateChanged;
    }

    public interface IReduxDispatcher
    {
        /// <summary>
        /// Event that signals that state has changed.
        /// </summary>
        /// <remarks>Runs on given TaskScheduler.</remarks>
        event EventHandler<StateChangedEventArgs> StateChanged;
        /// <summary>
        /// Event that signals that one ore more actions have been replayed.
        /// </summary>
        /// <remarks>Runs on given TaskScheduler.</remarks>
        event EventHandler<RepliedActionsEventArgs> RepliedActions;
        /// <summary>
        /// Dispatches given action.
        /// </summary>
        /// <param name="action">An <see cref="ReduxAction"/> to be dispatched.</param>
        /// <remarks>All actions have to be dispatched through this method.</remarks>
        void Dispatch(ReduxAction action);
        /// <summary>
        /// Starts the processor.
        /// </summary>
        /// <remarks>If processor is already running, an <see cref="Exception"/> is thrown.</remarks>
        void Start();
        /// <summary>
        /// Gets whether processor is running.
        /// </summary>
        bool IsProcessorRunning { get; }
        /// <summary>
        /// Stops processor if it is running. Does nothing otherwise.
        /// </summary>
        /// <returns>A task representing the stop process.</returns>
        Task StopAsync();
        ///// <summary>
        ///// Current state.
        ///// </summary>
        //object State { get; }
        /// <summary>
        /// Resets state to given <paramref name="newState"/>.
        /// </summary>
        /// <returns>An instance of <see cref="Task"/>.</returns>
        Task ResetStateAsync(object newState);
        /// <summary>
        /// Resets state to initial state.
        /// </summary>
        /// <returns>A task representing the reset process.</returns>
        Task ResetToInitialStateAsync();
        /// <summary>
        /// Replies a given set on actions.
        /// </summary>
        /// <param name="actions">Actions to be replayed.</param>
        /// <param name="progress">Progress feedback</param>
        /// <param name="ct">A cancellation token that can be used to cancel the work.</param>
        /// <returns>A task representing the replay process.</returns>
        /// <remarks>This method is used for replying back a current set of actions. State change notification happens only after last 
        /// action.
        /// Throws an exception if processor is running.</remarks>
        Task ReplayActionsAsync(ReduxAction[] actions, IProgress<int> progress, CancellationToken ct);
    }
}
