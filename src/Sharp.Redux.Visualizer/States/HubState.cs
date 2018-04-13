using Righthand.Immutable;
using Sharp.Redux.Shared.Models;

namespace Sharp.Redux.Visualizer.States
{
    public readonly struct HubState
    {
        public SessionInfo[] Sessions { get; }

        public HubState(SessionInfo[] sessions)
        {
            Sessions = sessions;
        }

        public HubState Clone(Param<SessionInfo[]>? sessions = null)
        {
            return new HubState(sessions.HasValue ? sessions.Value.Value : Sessions);
        }
    }
}
