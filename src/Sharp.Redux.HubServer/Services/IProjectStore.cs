using Sharp.Redux.HubServer.Data;
using System;

namespace Sharp.Redux.HubServer.Services
{
    public interface IProjectStore
    {
        void AddProject(SharpReduxProject project);
        SharpReduxProject[] GetUserProjects(string userId);
        SharpReduxProject GetUserProject(string userId, Guid projectId);
        bool DoesExist(Guid id);
    }
}
