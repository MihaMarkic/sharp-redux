using LiteDB;
using Sharp.Redux.HubClient.Core;
using Sharp.Redux.HubClient.Services.Abstract;
using Sharp.Redux.Shared.Models;
using System;
using System.IO;

namespace Sharp.Redux.HubClient.Services.Implementation
{
    public class Persister : DisposableObject, IPersister
    {
        LiteDatabase db;
        LiteCollection<Step> steps;
        public void Start(string dataFile)
        {
            db = new LiteDatabase(dataFile);
            steps = db.GetCollection<Step>("steps");
        }
        public void Save(Step step)
        {
            steps.Insert(step);
        }
        public void Remove(Step[] stepsToDelete)
        {
            BsonArray ids = new BsonArray();
            foreach (var step in stepsToDelete)
            {
                ids.Add(step.Id);
            }
            int deleted = steps.Delete(Query.In(nameof(Step.Id), ids));
            if (deleted != ids.Count)
            {
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
