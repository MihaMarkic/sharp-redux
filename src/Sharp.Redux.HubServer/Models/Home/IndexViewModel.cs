using Sharp.Redux.HubServer.Data;

namespace Sharp.Redux.HubServer.Models.Home
{
    public readonly struct IndexViewModel
    {
        public bool IsSignedIn { get;  }
        public SharpReduxProject[] Projects { get; }
        public IndexViewModel(bool isSignedIn, SharpReduxProject[] projects)
        {
            IsSignedIn = isSignedIn;
            Projects = projects;
        }
    }
}
