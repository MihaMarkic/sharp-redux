using Newtonsoft.Json;
using Sharp.Redux.HubClient.Core;
using Sharp.Redux.HubClient.Models;
using Sharp.Redux.HubClient.Services.Abstract;
using Sharp.Redux.HubClient.Services.Implementation;
using Sharp.Redux.Shared.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sharp.Redux.HubClient
{
    public class SharpReduxSender: IDisposable
    {
        bool isDisposed;
        readonly string token;
        readonly IReduxDispatcher dispatcher;
        readonly SharpReduxSenderSettings settings;
        readonly ICommunicator communicator;
        public static SharpReduxSender Default { get; private set; }
        public static HubClientSettings Settings { get; } = new HubClientSettings();
        readonly CancellationTokenSource cts;
        Task processor;
        readonly BlockingCollection<Step> buffer;
        readonly IPersister persister;
        int counter = 0;
        readonly Session session;
        public SessionInfo SessionInfo { get; private set; }
        public static void Start(string token, Uri serverUri, IReduxDispatcher dispatcher, SessionInfo sessionInfo, SharpReduxSenderSettings settings)
        {
            if (Default != null)
            {
                throw new Exception("Sender already running");
            }
            Default = new SharpReduxSender(token, serverUri, dispatcher, sessionInfo, settings, settings.PersistData ? new Persister() : null);
            Default.Start();
        }
        internal SharpReduxSender(string token, Uri serverUri, IReduxDispatcher dispatcher, SessionInfo sessionInfo, SharpReduxSenderSettings settings, IPersister persister):
            this(token, serverUri, dispatcher,sessionInfo, settings, persister, new Communicator(token, serverUri, settings.WaitForConnection))
        {}
        internal SharpReduxSender(string token, Uri serverUri, IReduxDispatcher dispatcher, SessionInfo sessionInfo, SharpReduxSenderSettings settings, IPersister persister,
            ICommunicator communicator)
        {
            this.token = token;
            this.dispatcher = dispatcher;
            this.persister = persister;
            dispatcher.StateChanged += Dispatcher_StateChanged;
            this.settings = settings;
            cts = new CancellationTokenSource();
            buffer = new BlockingCollection<Step>();
            this.communicator = communicator;
            SessionInfo = sessionInfo;
            session = new Session
            {
                Id = Guid.NewGuid(),
                ClientDateTime = DateTimeOffset.Now,
                AppVersion = sessionInfo.AppVersion,
                UserName = sessionInfo.UserName
            };
            Logger.Log(LogLevel.Info, $"Created session {session.Id}");
        }
        bool IsPersisterRunning => persister?.IsRunning ?? false;
        public Task UpdateSessionInfoAsync(SessionInfo sessionInfo, CancellationToken ct)
        {
            SessionInfo = sessionInfo;
            session.AppVersion = sessionInfo.AppVersion;
            session.UserName = sessionInfo.UserName;
            Logger.Log(LogLevel.Info, $"Updates session with appVersion:{sessionInfo.AppVersion} and UserName:{sessionInfo.UserName}");
            // update
            return UploadSessionAsync(ct);
        }
        internal Task UploadSessionAsync(CancellationToken ct)
        {
            persister?.RegisterSession(session);
            return communicator.RegisterSessionAsync(session, ct);
        }
        void Dispatcher_StateChanged(object sender, StateChangedEventArgs e)
        {
            var step = CreateStepFromStateChange(session.Id, counter++, settings, e);
            persister?.Save(step);
            buffer.Add(step);
            Logger.Log(LogLevel.Debug, $"Got action {e.Action.GetType().Name}");
        }
        internal static Step CreateStepFromStateChange(Guid sessionId, int counter, SharpReduxSenderSettings settings, StateChangedEventArgs e)
        {
            return new Step
            {
                SessionId = sessionId,
                Id = counter++,
                ActionType = e.Action.GetType().FullName,
                Action = JsonConvert.SerializeObject(e.Action),
                State = settings.IncludeState ? e.State: null,
                Time = DateTimeOffset.Now
            };
        }

        public void Start()
        {
            if (processor != null)
            {
                Logger.Log(LogLevel.Error, "Processor already running");
                throw new Exception("Processor already running");
            }
            persister?.Start(settings.DataFile);
            Logger.Log(LogLevel.Debug, "Starting processor");
            processor = Task.Run(() => ProcessorAsync(cts.Token), cts.Token);
        }
        public bool IsRunning => processor != null;
        async Task ProcessorAsync(CancellationToken ct)
        {
            if (IsPersisterRunning)
            {
                await ProcessPersistedAsync(ct);
                persister.RegisterSession(session);
            }
            await UploadSessionAsync(ct);
            List<Step> steps = new List<Step>(settings.BatchSize);
            while (!ct.IsCancellationRequested)
            {
                Logger.Log(LogLevel.Debug, "Waiting for batch");
                if (WaitForBatch(buffer, steps, settings.BatchSize, settings.CollectionSpan, ct))
                {
                    Logger.Log(LogLevel.Info, $"Got batch of size {steps.Count}");
                    await UploadBatchAsync(steps.ToArray(), ct);
                    steps.Clear();
                }
            }
        }
        internal async Task UploadBatchAsync(Step[] steps, CancellationToken ct)
        {
            await communicator.UploadStepsAsync(steps, ct);
            persister?.Remove(steps);
            Logger.Log(LogLevel.Info, "Batch upload complete");
        }
        /// <summary>
        /// Process not uploaded data.
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        internal async Task ProcessPersistedAsync(CancellationToken ct)
        {
            var sessions = persister.GetSessions();
            foreach (var session in sessions)
            {
                Step[] steps;
                do
                {
                    steps = persister.GetStepsFromSession(session.Id, settings.BatchSize);
                    if (steps.Length > 0)
                    {
                        await communicator.UploadStepsAsync(steps, ct);
                        persister.Remove(steps);
                    }
                } while (steps.Length > 0);
                persister.RemoveSession(session.Id);
            }
        }
        internal static bool WaitForBatch(BlockingCollection<Step> buffer, List<Step> steps, int batchSize, TimeSpan waitSpan, CancellationToken ct)
        {
            try
            {
                Logger.Log(LogLevel.Debug, "Waiting for first action");
                steps.Add(buffer.Take(ct));
            }
            catch (OperationCanceledException)
            {
                // we are done
                return false;
            }
            using (var cts = new CancellationTokenSource(waitSpan))
            using (var linkedSource = CancellationTokenSource.CreateLinkedTokenSource(ct, cts.Token))
            {
                try
                {
                    while (steps.Count < batchSize)
                    {
                        steps.Add(buffer.Take(linkedSource.Token));
                        Logger.Log(LogLevel.Debug, "Action added to batch");
                    }
                }
                catch (OperationCanceledException)
                {
                    if (ct.IsCancellationRequested)
                    {
                        return false;
                    }
                    Logger.Log(LogLevel.Debug, $"Timeout expired for batch waiting, got {steps.Count}");
                }
            }
            return true;
        }
        public async Task StopAsync()
        {
            Logger.Log(LogLevel.Info, "Stopping");
            dispatcher.StateChanged -= Dispatcher_StateChanged;
            cts.Cancel();
            await processor;
            processor = null;
            Logger.Log(LogLevel.Info, "Stopped");

        }

        public void Dispose()
        {
            if (!isDisposed)
            {
                var ignore = StopAsync();
                persister?.Dispose();
                isDisposed = true;
                communicator.Dispose();
            }
        }
    }
}
