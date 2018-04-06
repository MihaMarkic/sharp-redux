using LiteDB;
using Sharp.Redux.Shared.Models;
using System;
using System.Linq;

namespace Sharp.Redux.HubServer.Services
{
    public class SessionStore : ISessionStore
    {
        readonly LiteCollection<Session> sessions;
        readonly IProjectStore projectStore;
        public SessionStore(LiteDatabase db, IProjectStore projectStore)
        {
            this.projectStore = projectStore;

            sessions = db.GetCollection<Session>();
            sessions.EnsureIndex(u => u.Id, unique: true);
            sessions.EnsureIndex(u => u.ClientDateTime);
            sessions.EnsureIndex(u => u.ProjectId);
        }
        public void AddOrUpdate(Session session)
        {
            if (!sessions.Upsert(session))
            {
                throw new Exception("Failed upserting session");
            }
        }
        public Session[] GetLast(Guid projectId, int max)
        {
            return sessions.Find(s => s.ProjectId == projectId, limit: max).OrderByDescending(s => s.ClientDateTime).ToArray();
        }
        public bool DoesExist(Guid id)
        {
            return sessions.FindOne(p => p.Id == id) != null;
        }
        public Session Get(string userId, Guid sessionId)
        {
            var session = sessions.FindById(sessionId);
            if (session != null)
            {
                if (projectStore.GetUserProject(userId, session.ProjectId) != null)
                {
                    return session;
                }
            }
            return null;
        }
    }
}
