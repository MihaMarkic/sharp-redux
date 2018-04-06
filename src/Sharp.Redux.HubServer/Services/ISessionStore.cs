using Sharp.Redux.Shared.Models;
using System;
using System.Linq;

namespace Sharp.Redux.HubServer.Services
{
    public interface ISessionStore
    {
        void AddOrUpdate(Session session);
        Session[] GetLast(Guid projectId, int max);
        bool DoesExist(Guid id);
        Session Get(string userId, Guid sessionId);
    }
}
