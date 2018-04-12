using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Sharp.Redux.HubServer.Data;
using Sharp.Redux.HubServer.Models;
using Sharp.Redux.HubServer.Models.Home;
using Sharp.Redux.HubServer.Services;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Sharp.Redux.HubServer.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        readonly ISessionStore sessionStore;
        readonly IStepStore stepStore;
        readonly ITokenStore tokenStore;
        public HomeController(IProjectStore projectStore, ISessionStore sessionStore, IStepStore stepStore, ITokenStore tokenStore,
            UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager):
            base(userManager, signInManager, projectStore)
        {
            this.sessionStore = sessionStore;
            this.stepStore = stepStore;
            this.tokenStore = tokenStore;
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            SharpReduxProject[] projects;
            bool isSignedIn = signInManager.IsSignedIn(User);
            if (isSignedIn)
            {
                try
                {
                    var user = await GetUserAsync();
                    projects = projectStore.GetUserProjects(user.Id);
                }
                catch (Exception ex)
                {
                    await signInManager.SignOutAsync();
                    projects = new SharpReduxProject[0];
                }
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
            var sessions = (from s in sessionStore.GetLast(id, 10)
                                select new SessionViewModel(s, stepStore.CountForSession(s.Id))
                            ).ToArray();
            var tokens = tokenStore.GetForProject(id);
            return View(new ProjectDetailsViewModel(project.Id, project.Description, sessions, tokens));
        }
        [HttpGet]
        public async Task<IActionResult> SessionDetails(Guid id)
        {
            var user = await GetUserAsync();
            var session = sessionStore.Get(user.Id, id);
            if (session == null)
            {
                throw new ArgumentException($"Couldn't find session {id}");
            }
            var steps = (from s in stepStore.GetLastBatch(id, 10)
                            select new StepViewModel(s)
                            ).ToArray();
            return View(new SessionDetailsViewModel(session, steps));
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateReadToken(CreateTokenViewModel model)
        {
            tokenStore.AddReadToken(model.ProjectId);
            return RedirectToAction(nameof(ProjectDetails), new { Id = model.ProjectId });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateWriteToken(CreateTokenViewModel model)
        {
            tokenStore.AddWriteToken(model.ProjectId);
            return RedirectToAction(nameof(ProjectDetails), new { Id = model.ProjectId });
        }
    }
}
