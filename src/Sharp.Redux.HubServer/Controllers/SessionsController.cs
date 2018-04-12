using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Sharp.Redux.HubServer.Authentication;
using Sharp.Redux.HubServer.Models;
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
        readonly IStepStore stepStore;
        public SessionsController(ISessionStore sessionStore, IProjectStore projectStore, IStepStore stepStore,
            UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager) :
            base(userManager, signInManager, projectStore)
        {
            this.sessionStore = sessionStore;
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
        [HttpPost]
        [AllowAnonymous]
        [Route("[controller]/list")]
        [Authorize(AuthenticationSchemes = ReduxTokenAuthenticationOptions.AuthenticationScheme)]
        public IActionResult List([FromBody]SessionsFilter filter)
        {
            var tokenVerificationResult = CheckForPermissions(ReduxClaim.IsRead);
            if (tokenVerificationResult.StatusCode.HasValue)
            {
                return StatusCode((int)tokenVerificationResult.StatusCode.Value, tokenVerificationResult.Description);
            }
            var sessions = sessionStore.GetFiltered(tokenVerificationResult.ProjectId, filter);
            var result = sessions.Select(s => 
                new SessionInfo
                {
                    Id = s.Id,
                    ClientDateTime = s.ClientDateTime,
                    AppVersion = s.AppVersion,
                    UserName = s.UserName,
                    ActionsCount = stepStore.CountForSession(s.Id),
                    FirstActionDate = stepStore.GetFirst(s.Id)?.Time,
                    LastActionDate = stepStore.GetLast(s.Id)?.Time,
                }).ToArray();
            return Ok(result);
        }
        //[HttpGet]
        //[AllowAnonymous]
        //[Route("projects/list")]
        //[Authorize(AuthenticationSchemes = ReduxTokenAuthenticationOptions.AuthenticationScheme)]
        //public IActionResult Get(int? max = 10)
        //{
        //    var projectId = Guid.Parse(User.Claims.Single(c => c.Type == ReduxClaim.ProjectId).Value);

        //    if (!User.HasClaim(c => c.Type == ReduxClaim.IsRead))
        //    {
        //        return StatusCode((int)HttpStatusCode.Unauthorized, "Not allowed");
        //    }
        //    if (!projectStore.DoesExist(projectId))
        //    {
        //        return StatusCode((int)HttpStatusCode.Forbidden, "Project doesn't exist");
        //    }
        //    var sessions = from s in sessionStore.GetLast(projectId, max.Value)
        //                   select new SessionViewModel(s, stepStore.CountForSession(s.Id));
        //    return Ok(sessions);
        //}
    }
}
