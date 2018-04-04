using Sharp.Redux.Shared.Models;
using System;

namespace Sharp.Redux.HubServer.Models.Home
{
    public readonly struct ProjectDetailsViewModel
    {
        public Guid Id { get; }
        public string Description { get; }
        public Session[] Sessions { get; }

        public ProjectDetailsViewModel(Guid id, string description, Session[] sessions)
        {
            Id = id;
            Description = description;
            Sessions = sessions;
        }
    }
}
