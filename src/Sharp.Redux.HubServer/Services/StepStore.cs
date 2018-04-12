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
            steps.EnsureIndex(u => u.Id);
            steps.EnsureIndex(u => u.SessionId);
        }
        public void AddOrUpdate(Step step)
        {
            steps.Upsert(step);
        }
        public Step[] GetLastBatch(Guid sessionId, int max)
        {
            return steps.Find(s => s.SessionId == sessionId, limit: max).OrderByDescending(s => s.Id).ToArray();
        }
        public Step GetFirst(Guid sessionId)
        {
            return steps.Find(s => s.SessionId == sessionId).OrderBy(s => s.Id).FirstOrDefault();
        }
        public Step GetLast(Guid sessionId)
        {
            return steps.Find(s => s.SessionId == sessionId).OrderByDescending(s => s.Id).FirstOrDefault();
        }
        public int CountForSession(Guid sessionId)
        {
            return steps.Count(s => s.SessionId == sessionId);
        }
        /// <summary>
        /// Gets a filtered lists of steps for a given session.
        /// List won't include <see cref="Step.State"/>.
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public Step[] GetFiltered(Guid sessionId, StepsFilter filter)
        {
            return (from s in steps.Find(s => s.SessionId == sessionId, limit: filter.MaxCount ?? int.MaxValue)
                    select new Step
                    {
                        Id = s.Id,
                        Action = s.Action,
                        ActionType = s.ActionType,
                        SessionId = s.SessionId,
                        Time = s.Time
                    }).ToArray();
        }
    }
}
