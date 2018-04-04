using Sharp.Redux.Shared.Models;
using System;

namespace Sharp.Redux.HubServer.Models.Home
{
    public readonly struct ProjectDetailsViewModel
    {
        public Guid Id { get; }
        public string Description { get; }
        public SessionViewModel[] Sessions { get; }

        public ProjectDetailsViewModel(Guid id, string description, SessionViewModel[] sessions)
        {
            Id = id;
            Description = description;
            Sessions = sessions;
        }
    }
    public readonly struct SessionViewModel
    {
        public Session Session { get; }
        public int StepCount { get; }
        public SessionViewModel(Session session, int stepCount)
        {
            Session = session;
            StepCount = stepCount;
        }
    }
}
