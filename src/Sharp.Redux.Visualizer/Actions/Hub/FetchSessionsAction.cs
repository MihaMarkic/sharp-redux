using Sharp.Redux.Shared.Models;
using Righthand.Immutable;

namespace Sharp.Redux.Visualizer.Actions.Hub
{
    public class FetchSessionsAction : ReduxAction
    {
        public SessionInfo[] Sessions { get; }

        public FetchSessionsAction(SessionInfo[] sessions) : base()
        {
            Sessions = sessions;
        }
        public FetchSessionsAction Clone(Param<SessionInfo[]>? sessions = null)
        {
            return new FetchSessionsAction(sessions.HasValue ? sessions.Value.Value : Sessions);
        }
    }
}
