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
    public class SharpReduxManager : IDisposable, ISharpReduxManager
    {
        bool isDisposed;
        readonly string uploadToken;
        readonly string downloadToken;
        readonly IReduxDispatcher dispatcher;
        readonly SharpReduxManagerSettings settings;
        readonly ICommunicator communicator;
        public static SharpReduxManager Default { get; private set; }
        public static HubClientSettings Settings { get; } = new HubClientSettings();
        readonly CancellationTokenSource cts;
        Task processor;
        readonly BlockingCollection<Step> buffer;
        readonly IPersister persister;
        int counter = 0;
        readonly Session session;
        public EnvironmentInfo EnvironmentInfo { get; private set; }
        public static void Start(string uploadToken, string downloadToken, Uri serverUri, IReduxDispatcher dispatcher, EnvironmentInfo environmentInfo, SharpReduxManagerSettings settings)
        {
            if (Default != null)
            {
                throw new Exception("Sender already running");
            }
            Default = new SharpReduxManager(uploadToken, downloadToken, serverUri, dispatcher, environmentInfo, settings, settings.PersistData ? new Persister() : null);
            Default.Start();
        }
        internal SharpReduxManager(string uploadToken, string downloadToken, Uri serverUri, IReduxDispatcher dispatcher, EnvironmentInfo environmentInfo, SharpReduxManagerSettings settings, IPersister persister) :
            this(uploadToken, downloadToken, serverUri, dispatcher, environmentInfo, settings, persister, new Communicator(uploadToken, downloadToken, serverUri, settings.WaitForConnection))
        { }
        internal SharpReduxManager(string uploadToken, string downloadToken, Uri serverUri, IReduxDispatcher dispatcher, EnvironmentInfo environmentInfo, SharpReduxManagerSettings settings, IPersister persister,
            ICommunicator communicator)
        {
            this.uploadToken = uploadToken;
            this.downloadToken = downloadToken;
            this.dispatcher = dispatcher;
            this.persister = persister;
            dispatcher.StateChanged += Dispatcher_StateChanged;
            this.settings = settings;
            cts = new CancellationTokenSource();
            buffer = new BlockingCollection<Step>();
            this.communicator = communicator;
            EnvironmentInfo = environmentInfo;
            session = new Session
            {
                Id = Guid.NewGuid(),
                ClientDateTime = DateTimeOffset.Now,
                AppVersion = environmentInfo.AppVersion,
                UserName = environmentInfo.UserName
            };
            Logger.Log(LogLevel.Info, $"Created session {session.Id}");
        }
        bool IsPersisterRunning => persister?.IsRunning ?? false;
        public Task UpdateEnvironmentInfoAsync(EnvironmentInfo environmentInfo, CancellationToken ct)
        {
            EnvironmentInfo = environmentInfo;
            session.AppVersion = environmentInfo.AppVersion;
            session.UserName = environmentInfo.UserName;
            Logger.Log(LogLevel.Info, $"Updates session with appVersion:{environmentInfo.AppVersion} and UserName:{environmentInfo.UserName}");
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
        internal static Step CreateStepFromStateChange(Guid sessionId, int counter, SharpReduxManagerSettings settings, StateChangedEventArgs e)
        {
            return new Step
            {
                SessionId = sessionId,
                Id = counter++,
                ActionType = e.Action.GetType().FullName,
                Action = JsonConvert.SerializeObject(e.Action),
                State = settings.IncludeState ? e.State : null,
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
        public Task<Session[]> GetSessionsAsync(SessionsFilter filter, CancellationToken ct)
        {
            return communicator.PostAsync<SessionsFilter, Session[]>("sessions/list", filter, ct);
        }

        public Task<Step[]> GetStepsAsync(string sessionId, StepsFilter filter, CancellationToken ct)
        {
            return communicator.PostAsync<StepsFilter, Step[]>($"sessions/{sessionId}/steps/list", filter, ct);
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
