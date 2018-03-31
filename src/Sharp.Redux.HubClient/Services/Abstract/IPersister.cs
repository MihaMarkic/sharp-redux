using Sharp.Redux.Shared.Models;
using System;

namespace Sharp.Redux.HubClient.Services.Abstract
{
    public interface IPersister : IDisposable
    {
        void Start(string dataFile);
        void Remove(Step[] steps);
        void Save(Step step);
        Session[] GetSessions();
        Step[] GetStepsFromSession(Guid sessionId, int maxCount);
        bool RemoveSession(Guid sessionId);
        Guid RegisterSession(Session session);
    }
}
