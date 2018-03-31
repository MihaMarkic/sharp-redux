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
        readonly string projectId;
        readonly IReduxDispatcher dispatcher;
        readonly SharpReduxSenderSettings settings;
        readonly ICommunicator communicator;
        public static SharpReduxSender Default { get; private set; }
        readonly CancellationTokenSource cts;
        Task processor;
        readonly BlockingCollection<Step> buffer;
        readonly IPersister persister;
        int counter = 1;
        readonly Session session;
        public SessionInfo SessionInfo { get; private set; }
        public static void Start(string projectId, Uri serverUri, IReduxDispatcher dispatcher, SessionInfo sessionInfo, SharpReduxSenderSettings settings)
        {
            if (Default != null)
            {
                throw new Exception("Sender already running");
            }
            Default = new SharpReduxSender(projectId, serverUri, dispatcher, sessionInfo, settings, settings.PersistData ? new Persister() : null);
            Default.Start();
        }
        internal SharpReduxSender(string projectId, Uri serverUri, IReduxDispatcher dispatcher, SessionInfo sessionInfo, SharpReduxSenderSettings settings, IPersister persister)
        {
            this.projectId = projectId;
            this.dispatcher = dispatcher;
            this.persister = persister;
            dispatcher.StateChanged += Dispatcher_StateChanged;
            this.settings = settings;
            cts = new CancellationTokenSource();
            buffer = new BlockingCollection<Step>();
            communicator = new Communicator(projectId, serverUri, settings.WaitForConnection);
            SessionInfo = sessionInfo;
            session = new Session { Id = Guid.NewGuid(), ClientDateTime = DateTimeOffset.Now, AppVersion = sessionInfo.AppVersion, UserName = sessionInfo.UserName };
        }
        public Task UpdateSessionInfoAsync(SessionInfo sessionInfo, CancellationToken ct)
        {
            SessionInfo = sessionInfo;
            session.AppVersion = sessionInfo.AppVersion;
            session.UserName = sessionInfo.UserName;
            // update
            persister?.RegisterSession(session);
            return communicator.RegisterSessionAsync(session, ct);
        }
        void Dispatcher_StateChanged(object sender, StateChangedEventArgs e)
        {
            var step = new Step
            {
                SessionId = session.Id,
                Id = counter++,
                Action = e.Action,
                State = e.State,
                Time = DateTimeOffset.Now
            };
            if (settings.PersistData)
            {
                persister.Save(step);
            }
            buffer.Add(step);
        }

        public void Start()
        {
            if (processor != null)
            {
                throw new Exception("Processor already running");
            }
            persister?.Start(settings.DataFile);
            processor = Task.Run(() => ProcessorAsync(cts.Token), cts.Token);
        }
        public bool IsRunning => processor != null;
        async Task ProcessorAsync(CancellationToken ct)
        {
            if (settings.PersistData)
            {
                await ProcessPersistedAsync(ct);
                persister.RegisterSession(session);
            }
            List<Step> steps = new List<Step>(settings.BatchSize);
            while (!ct.IsCancellationRequested)
            {
                if (WaitForBatch(buffer, steps, settings.BatchSize, settings.CollectionSpan, ct))
                {
                    await UploadBatchAsync(steps.ToArray(), ct);
                }
            }
        }
        internal async Task UploadBatchAsync(Step[] steps, CancellationToken ct)
        {
            await communicator.UploadStepsAsync(steps, ct);
            persister?.Remove(steps);
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
            await Task.Delay(100, ct);
        }
        internal static bool WaitForBatch(BlockingCollection<Step> buffer, List<Step> steps, int batchSize, TimeSpan waitSpan, CancellationToken ct)
        {
            try
            {
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
                    }
                }
                catch (OperationCanceledException ex)
                {
                    if (ct.IsCancellationRequested)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        public async Task StopAsync()
        {
            dispatcher.StateChanged -= Dispatcher_StateChanged;
            cts.Cancel();
            await processor;
            processor = null;
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
