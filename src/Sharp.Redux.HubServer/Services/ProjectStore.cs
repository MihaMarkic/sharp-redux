using LiteDB;
using Sharp.Redux.HubServer.Data;
using System;
using System.Linq;

namespace Sharp.Redux.HubServer.Services
{
    public class ProjectStore : IProjectStore
    {
        readonly LiteCollection<SharpReduxProject> projects;
        public ProjectStore(LiteDatabase db)
        {
            projects = db.GetCollection<SharpReduxProject>();
            projects.EnsureIndex(u => u.Id, unique: true);
            projects.EnsureIndex(u => u.UserId);
        }
        public SharpReduxProject[] GetUserProjects(string userId)
        {
            return projects.FindAll().OrderBy(p => p.Created).ToArray();
        }
        public void AddProject(SharpReduxProject project)
        {
            projects.Insert(project);
        }
        public SharpReduxProject GetUserProject(string userId, Guid projectId)
        {
            var project = projects.FindById(projectId);
            if (project?.UserId == userId)
            {
                return project;
            }
            else
            {
                return null;
            }
        }
        public bool DoesExist(Guid id)
        {
            return projects.FindOne(p => p.Id == id) != null;
        }
    }
}
