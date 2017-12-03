using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Sharp.Redux
{
    public class ReduxDispatcher<TState, TReducer>: IReduxDispatcher<TState>
        where TReducer: IReduxReducer<TState>
    {
        private readonly TReducer reducer;
        public event EventHandler<StateChangedEventArgs> StateChanged;
        private readonly BlockingCollection<ReduxAction> queue = new BlockingCollection<ReduxAction>();
        public TState State { get; private set; }
        object IReduxDispatcher.State => State;
        private TaskFactory notificationFactory;
        private CancellationTokenSource cts;
        private Task processor;
        public ReduxDispatcher(TState initialState, TReducer reducer, TaskScheduler notificationScheduler)
        {
            State = initialState;
            this.reducer = reducer;
            notificationFactory = new TaskFactory(notificationScheduler);
        }

        public void Start()
        {
            if (processor != null)
            {
                throw new Exception("Dispatcher already running");
            }
            cts = new CancellationTokenSource();
            processor = Task.Run(() => ProcessorAsync(cts.Token));
        }

        public bool IsProcessorRunning => processor != null;

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
            State = newState;
        }
        /// <summary>
        /// Replies a given set on actions.
        /// </summary>
        /// <param name="actions"></param>
        /// <param name="progress">Progress feedback</param>
        /// <param name="ct"></param>
        /// <returns>An instance of <see cref="Task"/>.</returns>
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
                return Task.Run(() => ReplyActionsCoreAsnyc(actions, progress, ct), ct);
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
        internal async Task ReplyActionsCoreAsnyc(ReduxAction[] actions, IProgress<int> progress, CancellationToken ct)
        {
            if (actions == null)
            {
                throw new ArgumentNullException(nameof(actions));
            }
            while (!ct.IsCancellationRequested)
            {
                var oldState = State;
                for(int i=0; i <actions.Length; i++)
                {
                    var action = actions[i];
                    State = await reducer.ReduceAsync(State, action, ct);
                    progress?.Report(i);
                }
                if (!ct.IsCancellationRequested && !ReferenceEquals(oldState, State))
                {
                    await notificationFactory.StartNew(() => OnStateChanged(new StateChangedEventArgs(actions[actions.Length - 1])));
                }
            }
        }
        public void Dispatch(ReduxAction action)
        {
            queue.Add(action);
        }

        internal async Task ProcessorAsync(CancellationToken ct)
        {
            try
            {
                while (!ct.IsCancellationRequested)
                {
                    ReduxAction action;
                    if (queue.TryTake(out action, -1, ct))
                    {
                        await ProcessActionAsync(action, ct: ct);
                    }
                }
            }
            catch (OperationCanceledException)
            {}
        }

        internal async Task ProcessActionAsync(ReduxAction action, CancellationToken ct)
        {
            var oldState = State;
            State = await reducer.ReduceAsync(State, action, CancellationToken.None);
            if (!ct.IsCancellationRequested && !ReferenceEquals(oldState, State))
            {
                await notificationFactory.StartNew(() => OnStateChanged(new StateChangedEventArgs(action)));
            }
        }

        protected virtual void OnStateChanged(StateChangedEventArgs e)
        {
            try
            {
                StateChanged?.Invoke(this, e);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed procesing state changed event: {ex.Message}");
            }
        }

        public void Dispose()
        {
            StopAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        }
    }
}
