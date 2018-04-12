using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Sharp.Redux.HubServer.Authentication;
using Sharp.Redux.HubServer.Models;
using Sharp.Redux.HubServer.Services;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Sharp.Redux.HubServer.Controllers
{
    public abstract class BaseController: Controller
    {
        protected readonly UserManager<ApplicationUser> userManager;
        protected readonly SignInManager<ApplicationUser> signInManager;
        protected readonly IProjectStore projectStore;
        public BaseController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IProjectStore projectStore)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.projectStore = projectStore;
        }
        protected async Task<ApplicationUser> GetUserAsync()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
            }
            return user;
        }
        public (Guid ProjectId, HttpStatusCode? StatusCode, string Description) CheckForPermissions(string claim)
        {
            var projectId = Guid.Parse(User.Claims.Single(c => c.Type == ReduxClaim.ProjectId).Value);
            if (!User.HasClaim(c => c.Type == claim))
            {
                return (projectId, HttpStatusCode.Unauthorized, "Upload not allowed");
            }
            if (!projectStore.DoesExist(projectId))
            {
                return (projectId, HttpStatusCode.Forbidden, "Project doesn't exist");
            }
            return (projectId, null, null);
        }
    }
}
