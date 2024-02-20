using AuthDemo.Cache.Interfaces;
using AuthDemo.Cache.Models;
using AuthDemo.Contracts.DataTransferObjects.Request;
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
        [HttpGet("GetTokensPerUser/{id}")]
        public async Task<IActionResult> GetTokensPerUser(long id)
        {
            var result = await _cacheService.GetAllResourcesPerObjectIdAsync<AccessToken>(CacheResources.UserToken, id.ToString());
            return Ok(result);
        }

        [Authorize(Policy = Policies.Roles.Admin)]
        [HttpGet("RemoveTokenPerUser")]
        public async Task<IActionResult> RemoveTokenPerUser(AuthTokenRemoveRequest request)
        {
            var result = await _cacheService.RemoveResourcePerObjectIdAsync(CacheResources.UserToken, request.TokenId, request.UserId.ToString());
            return Ok(result);
        }

        [Authorize(Policy = Policies.Roles.Admin)]
        [HttpGet("RemoveAllTokensPerUser/{id}")]
        public async Task<IActionResult> RemoveAllTokensPerUser(long id)
        {
            var result = await _cacheService.RemoveAllResourcesPerObjectIdAsync(CacheResources.UserToken, id.ToString());
            return Ok(result);
        }
    }
}
