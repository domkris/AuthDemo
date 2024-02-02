using AuthDemo.Domain;
using AuthDemo.Infrastructure.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static AuthDemo.Domain.Cache.CacheKeys;
using Policies = AuthDemo.Security.Authorization.AuthDemoPolicies;

namespace AuthDemo.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly ISystemCache _systemCache;
        private readonly UserManager<User> _userManager;

        public UsersController(
            ISystemCache systemCache,
            UserManager<User> userManager)
        {
            _systemCache = systemCache;
            _userManager = userManager;
        }

        [Authorize(Policy = Policies.Roles.Admin)]
        [HttpPost("ToggleUserActivation/{id}")]
        public async Task<IActionResult> ToggleUserActivation(long id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return BadRequest("User does not exist");
            }
            user.IsActive = !user.IsActive;
            await _userManager.UpdateAsync(user);

            if (!user.IsActive)
            {
                // logout user from all sessions
                await _systemCache.RemoveAllResourcesPerObjectIdAsync(CacheResources.UserToken, id.ToString());
            }
            return Ok();
        }
    }
}
