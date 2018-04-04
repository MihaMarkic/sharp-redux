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
    public class HomeController : Controller
    {
        readonly UserManager<ApplicationUser> userManager;
        readonly IProjectStore projectStore;
        public HomeController(IProjectStore projectStore, UserManager<ApplicationUser> userManager)
        {
            this.projectStore = projectStore;
            this.userManager = userManager;
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
            }
            var projects = projectStore.GetUserProjects(user.Id);
            IndexViewModel model = new IndexViewModel { Projects = projects };
            return base.View(model);
        }
        [HttpGet]
        public IActionResult CreateProject()
        {
            return View(new NewProjectViewModel());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProject(NewProjectViewModel model)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
            }

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
