using AuthDemo.Contracts.DataTransferObjects.Request;
using AuthDemo.Contracts.DataTransferObjects.Response;
using AuthDemo.Security.Interfaces;
using AuthDemo.Web.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Policies = AuthDemo.Security.Authorization.AuthDemoPolicies;

namespace AuthDemo.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AuthTokensController : ControllerBase
    {
        private readonly ITokenService _tokenService;
      
        public AuthTokensController(
            ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        /// <summary>
        /// Used when access token is expired to generate a new one
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken(AuthTokenRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            (var accessToken, var refreshToken) = await _tokenService.VerifyAndGenerateTokens(request.AccessToken, request.RefreshToken);

            if(string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(refreshToken)) 
            {
                return Unauthorized("Invalid tokens");
            }

            return Ok(new AuthResponse { AccessToken = accessToken, RefreshToken = refreshToken });
        }

        [Authorize(Policy = Policies.Roles.Admin)]
        [HttpPut("InvalidateUserTokens/{id}")]
        public async Task<IActionResult> InvalidateUserTokens(long id)
        {
            var result = await _tokenService.InvalidateUserTokens(id, Constants.ReasonsOfRevoke.AdminRequestedInvalidationOfUserTokens);
            if (!result)
            {
                return BadRequest("Unable to invalidate tokens");
            }
            return Ok(result);
        }
    }
}
