using System;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Sharp.Redux.HubClient
{
    public class SharpReduxSender: IDisposable
    {
        bool isDisposed;
        readonly string projectId;
        readonly IReduxDispatcher dispatcher;
        readonly bool persistData;
        readonly HttpClient httpClient;
        public static SharpReduxSender Default { get; private set; }
        readonly CancellationTokenSource cts;
        Task processor;
        readonly BlockingCollection<string> queue;
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
            persistData = settings?.PersistData ?? false;
            httpClient = new HttpClient { BaseAddress = serverUri };
            cts = new CancellationTokenSource();
        }

        private void Dispatcher_StateChanged(object sender, StateChangedEventArgs e)
        {
            
        }

        public void Start()
        {
            processor = Task.Run(() => Processor(cts.Token), cts.Token);
        }

        async Task Processor(CancellationToken ct)
        {

        }

        public Task StopAsync()
        {
            cts.Cancel();
            return processor;
        }

        public void Dispose()
        {
            if (!isDisposed)
            {
                var ignore = StopAsync();
                isDisposed = true;
                dispatcher.StateChanged -= Dispatcher_StateChanged;
                httpClient.Dispose();
            }
        }
    }
}
