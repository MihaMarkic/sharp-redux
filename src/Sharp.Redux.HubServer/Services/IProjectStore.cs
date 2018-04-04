using Sharp.Redux.HubServer.Data;

namespace Sharp.Redux.HubServer.Services
{
    public interface IProjectStore
    {
        void AddProject(SharpReduxProject project);
        SharpReduxProject[] GetUserProjects(string userId);
    }
}
