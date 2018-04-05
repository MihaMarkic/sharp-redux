﻿using LiteDB;
using Sharp.Redux.HubClient.Core;
using Sharp.Redux.HubClient.Services.Abstract;
using Sharp.Redux.Shared.Models;
using System;
using System.IO;
using System.Linq;

namespace Sharp.Redux.HubClient.Services.Implementation
{
    public class Persister : DisposableObject, IPersister
    {
        LiteDatabase db;
        LiteCollection<Step> steps;
        LiteCollection<Session> sessions;
        public void Start(string dataFile)
        {
            Core.Logger.Log(LogLevel.Debug, "Starting persister");
            try
            {
                db = new LiteDatabase(dataFile);
            }
            catch (Exception ex)
            {
                Core.Logger.Log(LogLevel.Error, ex, "Failed creating database, persister won't work");
                throw;
            }
            Init();
            Core.Logger.Log(LogLevel.Info, "Persister start is complete");
        }
        public bool IsRunning => db != null;
        public void Start(Stream stream)
        {
            db = new LiteDatabase(stream, disposeStream: true);
            Init();
        }
        void Init()
        {
            steps = db.GetCollection<Step>("steps");
            steps.EnsureIndex(s => s.Id);
            sessions = db.GetCollection<Session>("sessions");
            sessions.EnsureIndex(s => s.Id);
        }
        public Session[] GetSessions()
        {
            return sessions.FindAll().ToArray();
        }
        public Step[] GetStepsFromSession(Guid sessionId, int maxCount)
        {
            return steps.Find(s => s.SessionId == sessionId, 0, maxCount).ToArray();
        }
        public bool RemoveSession(Guid sessionId)
        {
            return sessions.Delete(s => s.Id == sessionId) == 1;
        }
        public Guid RegisterSession(Session session)
        {
            if (IsRunning)
            {
                sessions.Insert(session);
            }
            return session.Id;
        }
        public void Save(Step step)
        {
            if (IsRunning)
            {
                steps.Insert(step);
            }
        }
        public void Remove(Step[] stepsToDelete)
        {
            BsonArray ids = new BsonArray();
            foreach (var step in stepsToDelete)
            {
                ids.Add(step.Id);
            }
            int deleted = steps.Delete(Query.In("_id", ids));
            if (deleted != ids.Count)
            {
                Core.Logger.Log(LogLevel.Error, "Failed deleting steps in persister");
                throw new Exception("Failed to delete all items");
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
