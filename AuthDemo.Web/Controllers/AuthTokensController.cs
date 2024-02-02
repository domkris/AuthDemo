using AuthDemo.Domain;
using AuthDemo.Domain.Cache.CacheObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static AuthDemo.Domain.Cache.CacheKeys;
using Policies = AuthDemo.Security.Authorization.AuthDemoPolicies;

namespace AuthDemo.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AuthTokensController : ControllerBase
    {
        private readonly ISystemCache _systemCache;

        public AuthTokensController(
            ISystemCache systemCache)
        {
            _systemCache = systemCache;
        }

        [Authorize(Policy = Policies.Roles.Admin)]
        [HttpGet("{key}")]
        public async Task<IActionResult> Get(string key)
        {
            UserToken? userToken = await _systemCache.GetDataAsync<UserToken>(key);
            return Ok(userToken);
        }

        [Authorize(Policy = Policies.Roles.Admin)]
        [HttpGet("GetTokensPerUser/{id}")]
        public async Task<IActionResult> GetTokensPerUser(long id)
        {
            var result = await _systemCache.GetAllResourcesPerObjectIdAsync<UserToken>(CacheResources.UserToken, id.ToString());
            return Ok(result);
        }
    }
}
