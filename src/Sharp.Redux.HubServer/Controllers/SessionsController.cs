using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Sharp.Redux.HubServer.Authentication;
using Sharp.Redux.HubServer.Models;
using Sharp.Redux.HubServer.Models.Home;
using Sharp.Redux.HubServer.Services;
using Sharp.Redux.Shared.Models;
using System;
using System.Linq;
using System.Net;

namespace Sharp.Redux.HubServer.Controllers
{
    public class SessionsController : BaseController
    {
        readonly ISessionStore sessionStore;
        readonly IProjectStore projectStore;
        readonly IStepStore stepStore;
        public SessionsController(ISessionStore sessionStore, IProjectStore projectStore, IStepStore stepStore,
            UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager) :
            base(userManager, signInManager)
        {
            this.sessionStore = sessionStore;
            this.projectStore = projectStore;
            this.stepStore = stepStore;
        }
        [HttpPost]
        [AllowAnonymous]
        [Route("[controller]")]
        [Authorize(AuthenticationSchemes = ReduxTokenAuthenticationOptions.AuthenticationScheme)]
        public IActionResult CreateOrUpdate([FromBody]Session session)
        {
            var projectId = Guid.Parse(User.Claims.Single(c => c.Type == ReduxClaim.ProjectId).Value);
            if (!User.HasClaim(c => c.Type == ReduxClaim.IsWrite))
            {
                return StatusCode((int)HttpStatusCode.Unauthorized, "Upload not allowed");
            }
            if (!projectStore.DoesExist(projectId))
            {
                return StatusCode((int)HttpStatusCode.Forbidden, "Project doesn't exist");
            }
            session.ProjectId = projectId;
            sessionStore.AddOrUpdate(session);
            return Ok();
        }
        [HttpGet]
        [AllowAnonymous]
        [Route("projects/list")]
        [Authorize(AuthenticationSchemes = ReduxTokenAuthenticationOptions.AuthenticationScheme)]
        public IActionResult Get(int? max = 10)
        {
            var projectId = Guid.Parse(User.Claims.Single(c => c.Type == ReduxClaim.ProjectId).Value);
            
            if (!User.HasClaim(c => c.Type == ReduxClaim.IsRead))
            {
                return StatusCode((int)HttpStatusCode.Unauthorized, "Not allowed");
            }
            if (!projectStore.DoesExist(projectId))
            {
                return StatusCode((int)HttpStatusCode.Forbidden, "Project doesn't exist");
            }
            var sessions = from s in sessionStore.GetLast(projectId, max.Value)
                           select new SessionViewModel(s, stepStore.CountForSession(s.Id));
            return Ok(sessions);
        }
    }
}
