using Sharp.Redux.HubClient.Models;
using Sharp.Redux.Shared.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sharp.Redux.HubClient
{
    public interface ISharpReduxManager
    {
        Task<Session[]> GetSessionsAsync(SessionsFilter filter, CancellationToken ct);
        Task<Step[]> GetStepsAsync(string sessionId, StepsFilter filter, CancellationToken ct);
        void Start();
        Task StopAsync();
        Task UpdateEnvironmentInfoAsync(EnvironmentInfo environmentInfo, CancellationToken ct);

        EnvironmentInfo EnvironmentInfo { get; }
        bool IsRunning { get; }
    }
}
