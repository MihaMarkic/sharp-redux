﻿using System;
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
        public event EventHandler<RepliedActionsEventArgs> RepliedActions;
        private readonly BlockingCollection<ReduxAction> queue = new BlockingCollection<ReduxAction>();
        public TState State { get; private set; }
        public TState InitialState { get; }
        object IReduxDispatcher.State => State;
        private TaskFactory notificationFactory;
        private CancellationTokenSource cts;
        private Task processor;
        public ReduxDispatcher(TState initialState, TReducer reducer, TaskScheduler notificationScheduler)
        {
            State = InitialState = initialState;
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
            await notificationFactory.StartNew(() => OnStateChanged(new StateChangedEventArgs(new StateResetAction())));
        }
        public Task ResetToInitialStateAsync()
        {
            return ResetStateAsync(InitialState);
        }
        /// <summary>
        /// Non typesafe method to reset state.
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
            var oldState = State;
            var replied = new RepliedAction[actions.Length];
            for (int i = 0; i < actions.Length; i++)
            {
                var action = actions[i];
                State = await reducer.ReduceAsync(State, action, ct);
                replied[i] = new RepliedAction(action, State);
                progress?.Report(i);
            }
            if (!ct.IsCancellationRequested && !ReferenceEquals(oldState, State))
            {
                await notificationFactory.StartNew(() => OnRepliedActions(new RepliedActionsEventArgs(replied)));
                await notificationFactory.StartNew(() => OnStateChanged(new StateChangedEventArgs(null)));
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
        protected virtual void OnRepliedActions(RepliedActionsEventArgs e) => RepliedActions?.Invoke(this, e);
        public void Dispose()
        {
            StopAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        }
    }
}