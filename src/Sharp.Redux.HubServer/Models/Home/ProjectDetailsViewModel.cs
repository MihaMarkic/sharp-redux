using Sharp.Redux.HubServer.Data;
using Sharp.Redux.Shared.Models;
using System;
using System.Linq;

namespace Sharp.Redux.HubServer.Models.Home
{
    public readonly struct ProjectDetailsViewModel
    {
        public Guid Id { get; }
        public string Description { get; }
        public SessionViewModel[] Sessions { get; }
        public SharpReduxToken[] Tokens { get; }
        public SharpReduxToken[] ReadTokens { get; }
        public SharpReduxToken[] WriteTokens { get; }
        public ProjectDetailsViewModel(Guid id, string description, SessionViewModel[] sessions, SharpReduxToken[] tokens)
        {
            Id = id;
            Description = description;
            Sessions = sessions;
            Tokens = tokens;
            ReadTokens = Tokens.Where(t => t.IsRead).ToArray();
            WriteTokens = Tokens.Where(t => t.IsWrite).ToArray();
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
