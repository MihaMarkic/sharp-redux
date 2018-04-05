using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Sharp.Redux.HubServer.Models;
using Sharp.Redux.HubServer.Services;
using Sharp.Redux.Shared.Models;
using System;

namespace Sharp.Redux.HubServer.Controllers
{
    [Authorize]
    public class SessionsController : BaseController
    {
        readonly ISessionStore sessionStore;
        readonly IProjectStore projectStore;
        public SessionsController(ISessionStore sessionStore, IProjectStore projectStore,
            UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager) :
            base(userManager, signInManager)
        {
            this.sessionStore = sessionStore;
            this.projectStore = projectStore;
        }
        [HttpPost]
        [AllowAnonymous]
        [Route("[controller]")]
        public IActionResult CreateOrUpdate([FromBody]Session session)
        {
            if (!projectStore.DoesExist(session.ProjectId))
            {
                throw new Exception("Project doesn't exist");
            }
            sessionStore.AddOrUpdate(session);
            return Ok();
        }
    }
}
