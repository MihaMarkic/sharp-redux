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
    public class StepsController : BaseController
    {
        readonly ISessionStore sessionStore;
        readonly IStepStore stepStore;
        public StepsController(ISessionStore sessionStore, IStepStore stepStore,
            UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager) :
            base(userManager, signInManager)
        {
            this.sessionStore = sessionStore;
            this.stepStore = stepStore;
        }
        [HttpPost]
        [AllowAnonymous]
        [Route("[controller]")]
        public IActionResult CreateOrUpdate(UploadBatch batch)
        {
            SaveSteps(batch.Steps);
            return Ok();
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
    }
}
