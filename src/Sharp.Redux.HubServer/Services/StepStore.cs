using LiteDB;
using Sharp.Redux.Shared.Models;
using System;
using System.Linq;

namespace Sharp.Redux.HubServer.Services
{
    public class StepStore : IStepStore
    {
        readonly LiteCollection<Step> steps;
        public StepStore(LiteDatabase db)
        {
            steps = db.GetCollection<Step>();
            steps.EnsureIndex(u => u.Id, unique: true);
            steps.EnsureIndex(u => u.SessionId);
        }
        public void AddOrUpdate(Step step)
        {
            if (!steps.Upsert(step))
            {
                throw new Exception("Failed upserting step");
            }
        }
        public Step[] GetLast(Guid sessionId, int max)
        {
            return steps.Find(s => s.SessionId == sessionId).OrderByDescending(s => s.Id).ToArray();
        }
        public int CountForSession(Guid sessionId)
        {
            return steps.Count(s => s.SessionId == sessionId);
        }
    }
}
