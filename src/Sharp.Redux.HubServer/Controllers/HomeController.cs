using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Sharp.Redux.HubServer.Data;
using Sharp.Redux.HubServer.Models;
using Sharp.Redux.HubServer.Models.Home;
using Sharp.Redux.HubServer.Services;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Sharp.Redux.HubServer.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        readonly IProjectStore projectStore;
        public HomeController(IProjectStore projectStore, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager):
            base(userManager, signInManager)
        {
            this.projectStore = projectStore;
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            SharpReduxProject[] projects;
            bool isSignedIn = signInManager.IsSignedIn(User);
            if (isSignedIn)
            {
                var user = await GetUserAsync();
                projects = projectStore.GetUserProjects(user.Id);
            }
            else
            {
                projects = new SharpReduxProject[0];
            }
            IndexViewModel model = new IndexViewModel (isSignedIn, projects);
            return base.View(model);
        }
        [HttpGet]
        public IActionResult CreateProject()
        {
            return View(new NewProjectViewModel());
        }
        [HttpGet]
        public async Task<IActionResult> ProjectDetails(Guid id)
        {
            var user = await GetUserAsync();
            var project = projectStore.GetUserProject(user.Id, id);
            if (project == null)
            {
                throw new ArgumentException($"Couldn't find project {id}");
            }
            return View(new ProjectDetailsViewModel(project.Id, project.Description, null));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProject(NewProjectViewModel model)
        {
            var user = await GetUserAsync();

            if (ModelState.IsValid)
            {
                var project = new SharpReduxProject
                {
                    Created = DateTimeOffset.Now,
                    Description = model.Description,
                    UserId = user.Id,
                };
                projectStore.AddProject(project);
            }
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
