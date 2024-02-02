using AuthDemo.Cache.Interfaces;
using AuthDemo.Cache.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static AuthDemo.Cache.Constants.CacheKeys;
using Policies = AuthDemo.Security.Authorization.AuthDemoPolicies;

namespace AuthDemo.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AuthTokensController : ControllerBase
    {
        private readonly ICacheService _cacheService;

        public AuthTokensController(
            ICacheService cacheService)
        {
            _cacheService = cacheService;
        }

        [Authorize(Policy = Policies.Roles.Admin)]
        [HttpGet("{key}")]
        public async Task<IActionResult> Get(string key)
        {
            UserToken? userToken = await _cacheService.GetDataAsync<UserToken>(key);
            return Ok(userToken);
        }

        [Authorize(Policy = Policies.Roles.Admin)]
        [HttpGet("GetTokensPerUser/{id}")]
        public async Task<IActionResult> GetTokensPerUser(long id)
        {
            var result = await _cacheService.GetAllResourcesPerObjectIdAsync<UserToken>(CacheResources.UserToken, id.ToString());
            return Ok(result);
        }
    }
}
