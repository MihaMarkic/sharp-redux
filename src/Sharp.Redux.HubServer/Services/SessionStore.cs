using LiteDB;
using Sharp.Redux.Shared.Models;
using System;
using System.Linq;

namespace Sharp.Redux.HubServer.Services
{
    public class SessionStore : ISessionStore
    {
        readonly LiteCollection<Session> sessions;
        public SessionStore(LiteDatabase db)
        {
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
            return sessions.Find(s => s.ProjectId == projectId).OrderByDescending(s => s.ClientDateTime).ToArray();
        }
        public bool DoesExist(Guid id)
        {
            return sessions.FindOne(p => p.Id == id) != null;
        }
    }
}
