using Sharp.Redux.Shared.Models;
using System;

namespace Sharp.Redux.HubServer.Services
{
    public interface IStepStore
    {
        void AddOrUpdate(Step step);
        Step[] GetLastBatch(Guid sessionId, int max);
        int CountForSession(Guid sessionId);
        Step[] GetFiltered(Guid sessionId, StepsFilter filter);
        Step GetFirst(Guid sessionId);
        Step GetLast(Guid sessionId);
    }
}
