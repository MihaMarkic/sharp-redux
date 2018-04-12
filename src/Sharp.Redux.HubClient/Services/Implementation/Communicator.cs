using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
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
        readonly string uploadToken;
        readonly string downloadToken;
        public static readonly JsonSerializerSettings SerializeSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.None,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };
        public Communicator(string uploadToken, string downloadToken, Uri serverUri, Func<CancellationToken, Task> waitForConnection)
        {
            this.serverUri = serverUri;
            this.waitForConnection = waitForConnection;
            this.uploadToken = uploadToken;
            this.downloadToken = downloadToken;
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
        static void AddToken(HttpRequestMessage request, string token)
        {
            request.Headers.Add("X-Token", $"ReduxToken {token}");
        }
        public async Task<TResult> PostAsync<TData, TResult>(string relativeUrl, TData data, CancellationToken ct)
        {
            string body = JsonConvert.SerializeObject(data);
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(serverUri, relativeUrl),
                Method = HttpMethod.Post,
                Content = new StringContent(body, System.Text.Encoding.UTF8, "application/json"),
            };
            AddToken(request, downloadToken);
            if (waitForConnection != null)
            {
                Logger.Log(LogLevel.Debug, "Waiting for connection");
                await waitForConnection(ct);
            }
            Logger.Log(LogLevel.Debug, "Post in progress");
            try
            {
                var response = await httpClient.SendAsync(request, ct);
                if (!response.IsSuccessStatusCode)
                {
                    string info = $"Response was not success {response.StatusCode}:{response.ReasonPhrase}";
                    Logger.Log(LogLevel.Warning, info);
                    throw new Exception(info);
                }
                else
                {
                    return await DeserializeResponseContentAsync<TResult>(response.Content, SerializeSettings, ct);
                }
            }
            catch (OperationCanceledException) when (!ct.IsCancellationRequested)
            {
                Logger.Log(LogLevel.Warning, $"Timeout while waiting for response");
                throw new Exception("Timeout while waiting for response");
            }
        }
        static async Task<TResult> DeserializeResponseContentAsync<TResult>(HttpContent content, JsonSerializerSettings serializerSettings, CancellationToken ct)
        {
            string responseBody = await content.ReadAsStringAsync();
            System.Diagnostics.Debug.WriteLine($"Deserializing {responseBody}");
            return DeserializeContent<TResult>(responseBody, serializerSettings);
        }
        static TResult DeserializeContent<TResult>(string body, JsonSerializerSettings serializerSettings)
        {
            var result = JsonConvert.DeserializeObject<TResult>(body, serializerSettings);
            return result;
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
                    AddToken(request, uploadToken);
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
