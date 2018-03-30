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
        Persister persister;
        public static void Start(string projectId, Uri serverUri, IReduxDispatcher dispatcher, SharpReduxSenderSettings settings)
        {
            Default = new SharpReduxSender(projectId, serverUri, dispatcher, settings);
            Default.Start();
        }
        private SharpReduxSender(string projectId, Uri serverUri, IReduxDispatcher dispatcher, SharpReduxSenderSettings settings)
        {
            this.projectId = projectId;
            this.dispatcher = dispatcher;
            dispatcher.StateChanged += Dispatcher_StateChanged;
            this.settings = settings;
            cts = new CancellationTokenSource();
            buffer = new BlockingCollection<Step>();
            communicator = new Communicator(projectId, serverUri, settings.WaitForConnection);
        }

        private void Dispatcher_StateChanged(object sender, StateChangedEventArgs e)
        {
            var step = new Step
            {
                Id = Guid.NewGuid(),
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
            if (settings.PersistData)
            {
                persister = new Persister();
                persister.Start();
            }
            processor = Task.Run(() => ProcessorAsync(cts.Token), cts.Token);
        }

        async Task ProcessorAsync(CancellationToken ct)
        {
            List<Step> steps = new List<Step>(settings.BatchSize);
            while (!ct.IsCancellationRequested)
            {
                if (WaitForBatch(buffer, steps, settings.BatchSize, settings.CollectionSpan, ct))
                {
                    await communicator.UploadStepsAsync(steps.ToArray(), ct);
                }
            }
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
                    if (ex.CancellationToken == ct)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        public Task StopAsync()
        {
            dispatcher.StateChanged -= Dispatcher_StateChanged;
            cts.Cancel();
            return processor;
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
