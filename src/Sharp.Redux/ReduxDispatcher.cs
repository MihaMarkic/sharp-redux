using Sharp.Redux.Actions;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Sharp.Redux
{
    /// <summary>
    /// Redux dispatcher. This class handles incoming actions sequentially and invokes <see cref="reducer"/> to process them.
    /// </summary>
    /// <typeparam name="TState">Type of the root state.</typeparam>
    /// <typeparam name="TReducer">Type of the reducer.</typeparam>
    public class ReduxDispatcher<TState, TReducer>: IReduxDispatcher<TState>
        where TReducer: IReduxReducer<TState>
    {
        /// <summary>
        /// The reducer for actions.
        /// </summary>
        private readonly TReducer reducer;
        /// <summary>
        /// Event that signals that state has changed.
        /// </summary>
        /// <remarks>Runs on given TaskScheduler.</remarks>
        public event EventHandler<StateChangedEventArgs<TState>> StateChanged;
        /// <summary>
        /// The non generic state changed event.
        /// </summary>
        EventHandler<StateChangedEventArgs> stateChanged;
        event EventHandler<StateChangedEventArgs> IReduxDispatcher.StateChanged { add { stateChanged += value; } remove { stateChanged -= value; } }
        /// <summary>
        /// Event that signals that one ore more actions have been replayed.
        /// </summary>
        /// <remarks>Runs on given TaskScheduler.</remarks>
        public event EventHandler<RepliedActionsEventArgs> RepliedActions;
        /// <summary>
        /// Queue for actions.
        /// </summary>
        private readonly BlockingCollection<ReduxAction> queue = new BlockingCollection<ReduxAction>();
        /// <summary>
        /// Current state.
        /// </summary>
        TState state;
        /// <summary>
        /// Initial state.
        /// </summary>
        public TState InitialState { get; }
        /// <summary>
        /// Task factory for invoking synchronized code (events).
        /// </summary>
        private TaskFactory notificationFactory;
        /// <summary>
        /// Provides cancellation for processor.
        /// </summary>
        private CancellationTokenSource cts;
        /// <summary>
        /// Action processor.
        /// </summary>
        /// <remarks>Runs in a background task.</remarks>
        private Task processor;
        /// <summary>
        /// Initialize a new instance of the <see cref="ReduxDispatcher{TState, TReducer}"/> that has initial state uses given reducer and runs synchronized event
        /// on <paramref name="notificationScheduler"/>.
        /// </summary>
        /// <param name="initialState">The initial state.</param>
        /// <param name="reducer">The reducer to use.</param>
        /// <param name="notificationScheduler">A scheduler for events.</param>
        public ReduxDispatcher(TState initialState, TReducer reducer, TaskScheduler notificationScheduler)
        {
            state = InitialState = initialState;
            this.reducer = reducer;
            notificationFactory = new TaskFactory(notificationScheduler);
        }
        /// <summary>
        /// Starts the processor.
        /// </summary>
        /// <remarks>If processor is already running, an <see cref="Exception"/> is thrown.</remarks>
        public void Start()
        {
            if (processor != null)
            {
                throw new Exception("Dispatcher already running");
            }
            cts = new CancellationTokenSource();
            processor = Task.Run(() => ProcessorAsync(cts.Token));
        }
        /// <summary>
        /// Gets whether processor is running.
        /// </summary>
        public bool IsProcessorRunning => processor != null;
        /// <summary>
        /// Stops processor if it is running. Does nothing otherwise.
        /// </summary>
        /// <returns>A task representing the stop process.</returns>
        public async Task StopAsync()
        {
            if (processor != null)
            {
                cts?.Cancel();
                await processor.ConfigureAwait(false);
                cts = null;
                processor = null;
            }
        }
        /// <summary>
        /// Resets state to given <paramref name="newState"/>.
        /// </summary>
        /// <returns>An instance of <see cref="Task"/>.</returns>
        public async Task ResetStateAsync(TState newState)
        {
            await StopAsync();
            state = newState;
            await notificationFactory.StartNew(st => OnStateChangedAsync(new StateChangedEventArgs<TState>(new StateResetAction(), (TState)st)), state).Unwrap();
        }
        /// <summary>
        /// Resets state to initial state.
        /// </summary>
        /// <returns>A task representing the reset process.</returns>
        public Task ResetToInitialStateAsync()
        {
            return ResetStateAsync(InitialState);
        }
        /// <summary>
        /// Non type safe method to reset state.
        /// </summary>
        /// <param name="newState"></param>
        /// <returns></returns>
        /// <remarks>Used by visualizer or other generic code.</remarks>
        Task IReduxDispatcher.ResetStateAsync(object newState)
        {
            return ResetStateAsync((TState)newState);
        }
        /// <summary>
        /// Replies a given set on actions.
        /// </summary>
        /// <param name="actions">Actions to be replayed.</param>
        /// <param name="progress">Progress feedback</param>
        /// <param name="ct">A cancellation token that can be used to cancel the work</param>
        /// <returns>A task representing the replay process.</returns>
        /// <remarks>This method is used for replying back a current set of actions. State change notification happens only after last 
        /// action.
        /// Throws an exception if processor is running.</remarks>
        public Task ReplayActionsAsync(ReduxAction[] actions, IProgress<int> progress, CancellationToken ct)
        {
            if (IsProcessorRunning)
            {
                throw new Exception("Can't reply actions when processor is running");
            }
            if (actions?.Length > 0)
            {
                return Task.Run(() => ReplyActionsCoreAsync(actions, progress, ct), ct);
            }
            else
            {
                return Task.FromResult(false);
            }
        }
        /// <summary>
        /// Core method that replies actions.
        /// </summary>
        /// <param name="actions"></param>
        /// <param name="progress">Progress feedback</param>
        /// <param name="ct"></param>
        /// <returns></returns>
        internal async Task ReplyActionsCoreAsync(ReduxAction[] actions, IProgress<int> progress, CancellationToken ct)
        {
            if (actions == null)
            {
                throw new ArgumentNullException(nameof(actions));
            }
            var oldState = state;
            var replied = new RepliedAction[actions.Length];
            for (int i = 0; i < actions.Length; i++)
            {
                var action = actions[i];
                state = await reducer.ReduceAsync(state, action, ct);
                replied[i] = new RepliedAction(action, state);
                progress?.Report(i);
            }
            if (!ct.IsCancellationRequested && !ReferenceEquals(oldState, state))
            {
                await notificationFactory.StartNew(() => OnRepliedActions(new RepliedActionsEventArgs(replied)));
                await notificationFactory.StartNew(st => OnStateChangedAsync(new StateChangedEventArgs<TState>(new ActionsRepliedAction(), (TState)st)), state).Unwrap();
            }
        }
        /// <summary>
        /// Dispatches given action.
        /// </summary>
        /// <param name="action">An <see cref="ReduxAction"/> to be dispatched.</param>
        /// <remarks>All actions have to be dispatched through this method.</remarks>
        public void Dispatch(ReduxAction action)
        {
            queue.Add(action);
        }
        /// <summary>
        /// The action processor. Handles the action queue and processes actions sequentially.
        /// </summary>
        /// <param name="ct">A cancellation token that can be used to cancel the work</param>
        /// <returns>A task that represents the processor.</returns>
        /// <remarks>Runs in a background thread.</remarks>
        internal async Task ProcessorAsync(CancellationToken ct)
        {
            try
            {
                await OnStateChangedAsync(new StateChangedEventArgs<TState>(new InitAction(), state));
                while (!ct.IsCancellationRequested)
                {
                    if (queue.TryTake(out var action, -1, ct))
                    {
                        await ProcessActionAsync(action, ct: ct);
                    }
                }
            }
            catch (OperationCanceledException)
            {}
        }
        /// <summary>
        /// Processes single action.
        /// </summary>
        /// <param name="action">An action to be processed.</param>
        /// <param name="ct">A cancellation token that can be used to cancel the work</param>
        /// <returns>A task that represents action process.</returns>
        internal async Task ProcessActionAsync(ReduxAction action, CancellationToken ct)
        {
            var oldState = state;
            state = await reducer.ReduceAsync(state, action, CancellationToken.None);
            if (!ct.IsCancellationRequested && !ReferenceEquals(oldState, state))
            {
                await notificationFactory.StartNew(st => OnStateChangedAsync(new StateChangedEventArgs<TState>(action, (TState)st)), state).Unwrap();
            }
        }
        readonly static Task defaultOnStateChangedResult = Task.FromResult(true);
        /// <summary>
        /// Raises <see cref="StateChanged"/> event. Client can add Tasks to event arguments for dispatcher to await their completion before 
        /// processing next action. See <see cref="StateChangedEventArgs.AddRunningTask(Task)"/> method.
        /// </summary>
        /// <param name="e">Arguments.</param>
        /// <remarks>Exceptions are swallowed.</remarks>
        /// <remarks>Runs on given scheduler thread. Fires both non-generic and generic (state type) events.</remarks>
        protected virtual Task OnStateChangedAsync(StateChangedEventArgs<TState> e)
        {
            try
            {
                stateChanged?.Invoke(this, e);
                StateChanged?.Invoke(this, e);
                // await client tasks if any
                if (e.RunningTasks?.Length > 0)
                {
                    return Task.WhenAll(e.RunningTasks);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed processing state changed event: {ex.Message}");
            }
            return defaultOnStateChangedResult;
        }
        /// <summary>
        /// Raises <see cref="RepliedAction"/> event.
        /// </summary>
        /// <param name="e"></param>
        /// <remarks>Runs on given scheduler thread.</remarks>
        protected virtual void OnRepliedActions(RepliedActionsEventArgs e) => RepliedActions?.Invoke(this, e);
        /// <summary>
        /// Releases all resources used by the current instance of the <see cref="ReduxDispatcher{TState, TReducer}"/> class.
        /// </summary>
        public void Dispose()
        {
            StopAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        }
    }
}
