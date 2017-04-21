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
        public event EventHandler StateChanged;
        private readonly BlockingCollection<ReduxAction> queue = new BlockingCollection<ReduxAction>();
        public TState State { get; private set; }
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

        public void Dispatch(ReduxAction action)
        {
            queue.Add(action);
        }

        public async Task ProcessorAsync(CancellationToken ct)
        {
            try
            {
                while (!ct.IsCancellationRequested)
                {
                    ReduxAction action;
                    if (queue.TryTake(out action, -1, ct))
                    {
                        await ProcessActionAsync(action, ct);
                    }
                }
            }
            catch (OperationCanceledException)
            {}
        }

        public async Task ProcessActionAsync(ReduxAction action, CancellationToken ct)
        {
            var oldState = State;
            State = await reducer.ReduceAsync(State, action, CancellationToken.None);
            if (!ct.IsCancellationRequested)
            {
                await notificationFactory.StartNew(() => OnStateChanged(EventArgs.Empty));
            }
        }

        protected virtual void OnStateChanged(EventArgs e)
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
