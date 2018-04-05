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
        readonly Uri serverUri;
        public Communicator(Guid projectId, Uri serverUri, Func<CancellationToken, Task> waitForConnection)
        {
            this.serverUri = serverUri;
            this.waitForConnection = waitForConnection;
            httpClient = new HttpClient();
        }
        public Task UploadStepsAsync(Step[] steps, CancellationToken ct)
        {
            Logger.Log(LogLevel.Info, $"Will upload {steps.Length} steps");
            var batch = new UploadBatch { Steps = steps };
            return UploadAsync("steps", batch, ct);
        }
        public Task RegisterSessionAsync(Session session, CancellationToken ct)
        {
            Logger.Log(LogLevel.Info, "Will upload session");
            return UploadAsync("sessions", session, ct);
        }
        async Task UploadAsync<T>(string relativeUrl, T data, CancellationToken ct)
        {
            try
            {
                HttpResponseMessage response;
                long retry = 0;
                bool success;
                do
                {
                    success = false;
                    if (retry > 0)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(5));
                        Logger.Log(LogLevel.Info, $"Uploading retry {retry}");
                    }
                    string body = JsonConvert.SerializeObject(data);
                    var request = new HttpRequestMessage
                    {
                        RequestUri = new Uri(serverUri, relativeUrl),
                        Method = HttpMethod.Post,
                        Content = new StringContent(body, System.Text.Encoding.UTF8, "application/json"),
                    };
                    if (waitForConnection != null)
                    {
                        Logger.Log(LogLevel.Debug, "Waiting for connection");
                        await waitForConnection(ct);
                    }
                    Logger.Log(LogLevel.Debug, "Uploading");
                    try
                    {
                        response = await httpClient.SendAsync(request, ct);
                        if (!response.IsSuccessStatusCode)
                        {
                            Logger.Log(LogLevel.Warning, $"Response was not success {response.StatusCode}:{response.ReasonPhrase}");
                        }
                        else
                        {
                            success = true;
                        }
                    }
                    catch (OperationCanceledException) when (!ct.IsCancellationRequested)
                    {
                        Logger.Log(LogLevel.Warning, $"Timeout while waiting for response");
                    }
                    catch (OperationCanceledException) when (ct.IsCancellationRequested)
                    {
                        Logger.Log(LogLevel.Info, "Sending was cancelled");
                    }
                    catch (HttpRequestException ex)
                    {
                        Logger.Log(LogLevel.Warning, $"Failed sending: {ex.Message}");
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(LogLevel.Error, ex, $"Failed sending");
                    }
                    retry++;
                } while (!success);
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                Logger.Log(LogLevel.Info, "Uploading was cancelled");
            }
            Logger.Log(LogLevel.Warning, $"Done uploading");
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
