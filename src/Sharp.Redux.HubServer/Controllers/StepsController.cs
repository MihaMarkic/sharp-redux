using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Sharp.Redux.HubServer.Authentication;
using Sharp.Redux.HubServer.Models;
using Sharp.Redux.HubServer.Services;
using Sharp.Redux.Shared.Models;
using System;

namespace Sharp.Redux.HubServer.Controllers
{
    [Authorize]
    public class StepsController : BaseController
    {
        readonly ISessionStore sessionStore;
        readonly IStepStore stepStore;
        public StepsController(ISessionStore sessionStore, IStepStore stepStore, IProjectStore projectStore,
            UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager) :
            base(userManager, signInManager, projectStore)
        {
            this.sessionStore = sessionStore;
            this.stepStore = stepStore;
        }
        [HttpPost]
        [AllowAnonymous]
        [Route("[controller]")]
        public IActionResult CreateOrUpdate([FromBody]UploadBatch batch)
        {
            if (batch?.Steps?.Length > 0)
            {
                SaveSteps(batch.Steps);
                return Ok();
            }
            else
            {
                return BadRequest("No steps");
            }
        }
        void SaveSteps(Step[] steps)
        {
            foreach (var step in steps)
            {
                if (!sessionStore.DoesExist(step.SessionId))
                {
                    throw new Exception($"Session {step.SessionId} doesn't exist");
                }
                stepStore.AddOrUpdate(step);
            }
        }
        [HttpPost]
        [AllowAnonymous]
        [Route("/sessions/{sessionId}/[controller]/list")]
        [Authorize(AuthenticationSchemes = ReduxTokenAuthenticationOptions.AuthenticationScheme)]
        public IActionResult List(Guid sessionId, [FromBody]StepsFilter filter)
        {
            var tokenVerificationResult = CheckForPermissions(ReduxClaim.IsRead);
            if (tokenVerificationResult.StatusCode.HasValue)
            {
                return StatusCode((int)tokenVerificationResult.StatusCode.Value, tokenVerificationResult.Description);
            }
            var steps = stepStore.GetFiltered(tokenVerificationResult.ProjectId, filter);
            return Ok(steps);
        }
    }
}
