using LiteDB;
using Sharp.Redux.HubClient.Core;
using Sharp.Redux.Shared.Models;

namespace Sharp.Redux.HubClient.Services.Implementation
{
    public class Persister: DisposableObject
    {
        LiteDatabase db;
        LiteCollection<Step> steps;
        long counter = 0;
        public void Start()
        {
            db = new LiteDatabase("steps.db");
            steps = db.GetCollection<Step>("steps");
        }
        public void Save(Step step)
        {
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
