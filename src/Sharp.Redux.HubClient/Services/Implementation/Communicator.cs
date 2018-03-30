using Newtonsoft.Json;
using Sharp.Redux.HubClient.Core;
using Sharp.Redux.HubClient.Services.Abstract;
using Sharp.Redux.Shared.Models;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Sharp.Redux.HubClient.Services.Implementation
{
    public class Communicator : DisposableObject, ICommunicator
    {
        readonly HttpClient httpClient;
        readonly Func<CancellationToken, Task> waitForConnection;
        public Communicator(string projectId, Uri serverUri, Func<CancellationToken, Task> waitForConnection)
        {
            httpClient = new HttpClient { BaseAddress = new Uri(serverUri, projectId) };
            this.waitForConnection = waitForConnection;
        }
        public async Task UploadStepsAsync(Step[] steps, CancellationToken ct)
        {
            HttpResponseMessage response;
            long retry = 0;
            do
            {
                if (retry > 0)
                {
                    await Task.Delay(TimeSpan.FromSeconds(5));
                }
                var batch = new UploadBatch { Steps = steps };
                string body = JsonConvert.SerializeObject(batch);
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    Content = new StringContent(body, System.Text.Encoding.UTF8, "application/json"),
                };
                if (waitForConnection != null)
                {
                    await waitForConnection(ct);
                }
                response = await httpClient.SendAsync(request, ct);
                retry++;
            } while (!response.IsSuccessStatusCode);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                httpClient.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
