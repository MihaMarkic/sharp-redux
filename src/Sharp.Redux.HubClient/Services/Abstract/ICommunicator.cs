using Sharp.Redux.Shared.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sharp.Redux.HubClient.Services.Abstract
{
    public interface ICommunicator: IDisposable
    {
        Task UploadStepsAsync(Step[] steps, CancellationToken ct);
    }
}
