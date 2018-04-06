using Sharp.Redux.Shared.Models;
using System;

namespace Sharp.Redux.HubServer.Models.Home
{
    public class SessionDetailsViewModel
    {
        public Session Session { get; }
        public StepViewModel[] Steps { get; }
        public SessionDetailsViewModel(Session session, StepViewModel[] steps)
        {
            Session = session;
            Steps = steps;
        }
    }

    public class StepViewModel
    {
        const string Actions = ".Actions.";
        public Step Step { get; }
        public StepViewModel(Step step)
        {
            Step = step;
        }
        public string NormalizedActionType
        {
            get
            {
                int actionsIndex = Step.ActionType.IndexOf(Actions);
                if (actionsIndex >= 0)
                {
                    return Step.ActionType.Substring(actionsIndex + Actions.Length);
                }
                else
                {
                    return Step.ActionType;
                }
            } 
        }
    }
}
