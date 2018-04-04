using Sharp.Redux.Shared.Models;
using System;
using System.Linq;

namespace Sharp.Redux.HubServer.Services
{
    public interface IStepStore
    {
        void AddOrUpdate(Step step);
        Step[] GetLast(Guid sessionId, int max);
        int CountForSession(Guid sessionId);
    }
}
