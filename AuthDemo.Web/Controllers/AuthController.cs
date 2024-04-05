using AuthDemo.Contracts.DataTransferObjects.Request;
using AuthDemo.Contracts.DataTransferObjects.Response;
using AuthDemo.Domain.Identity.Interfaces;
using AuthDemo.Infrastructure.LookupData;
using AuthDemo.Security.Interfaces;
using AuthDemo.Web.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Policies = AuthDemo.Security.Authorization.AuthDemoPolicies;

namespace AuthDemo.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AuthController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly IUserIdentityService _userIdentityService;

        public AuthController(
            ITokenService tokenService,
            IUserIdentityService userIdentityService)
        {
            _tokenService = tokenService;
            _userIdentityService = userIdentityService;
        }

       
        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> Login(AuthLoginRequest request)
        {
            bool rememberMe = false;
            bool lockoutOnFailure = true;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userIdentityService.FindByEmailAsync(request.Email);
            if (user == null) 
            {                 
                return BadRequest("Invalid login attempt");       
            }

            var result = await _userIdentityService.PasswordSignInAsync(user, request.Password, rememberMe, lockoutOnFailure);

            if(result.IsLockedOut)
            {
                return BadRequest("Too many unsuceesful login attempts, your account is temporarily locked. Please try again in 5 minutes.");
            }

            if (result.Succeeded)
            {
                (var accessToken, var refreshToken) = await _tokenService.GenerateTokens(user!);

                return Ok(new AuthResponse{ AccessToken = accessToken, RefreshToken = refreshToken });
            }

            return BadRequest("Invalid login attempt");
        }

        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            string? accessTokenId = User.FindFirstValue(JwtRegisteredClaimNames.Jti);
            if(accessTokenId == null)
            {
                return BadRequest("Invalid logout attempt");
            }

            long.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out long loggedInUserId);

            var result = await _tokenService.InvalidateUserTokensOnLogout(accessTokenId, loggedInUserId, Constants.ReasonsOfRevoke.UserRequestedLogout);

            if (!result)
            {
                return BadRequest("Unable to logout");
            }
            return Ok(new { message = "Logout successful" });
        }

        [HttpPost("LogoutAllSessions")]
        public async Task<IActionResult> LogoutAllSessions()
        {
            string? accessTokenId = User.FindFirstValue(JwtRegisteredClaimNames.Jti);
            if (accessTokenId == null)
            {
                return BadRequest("Invalid logout attempt");
            }

            long.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out long loggedInUserId);

            var result = await _tokenService.InvalidateUserTokens(loggedInUserId, Constants.ReasonsOfRevoke.UserRequestedInvalidationOfUserTokens);
            if (!result)
            {
                return BadRequest("Unable to logout");
            }
            return Ok(new { message = "Logout successful" });
        }


        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword(AuthPasswordChangeRequest request)
        {
            long.TryParse(User.FindFirstValue(ClaimTypes.Role), out long loggedInUserRole);
            long.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out long loggedInUserId);

            var dbUser = await _userIdentityService.FindByIdAsync(request.UserId);

            if (dbUser == null)
            {
                return BadRequest("User does not exist");
            }

            if (request.UserId != loggedInUserId && loggedInUserRole is not (long)Roles.Administrator)
            {
                return Forbid();
            }

            await _userIdentityService.ChangePasswordAsync(dbUser, request.CurrentPassword, request.NewPassword);

            // logout user from all sessions
            var result = await _tokenService.InvalidateUserTokens(loggedInUserId, Constants.ReasonsOfRevoke.UserChangedPassword);
            if (!result)
            {
                return BadRequest("Unable to invalidate tokens");
            }
            return Ok();
        }

        [HttpPost("ChangeEmail")]
        public async Task<IActionResult> ChangeEmail(AuthEmailChangeRequest request)
        {
            long.TryParse(User.FindFirstValue(ClaimTypes.Role), out long loggedInUserRole);
            long.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out long loggedInUserId);

            var dbUser = await _userIdentityService.FindByIdAsync(request.UserId);

            if (dbUser == null)
            {
                return BadRequest("User does not exist");
            }

            if (dbUser!.Id != loggedInUserId && loggedInUserRole is not (long)Roles.Administrator)
            {
                return Forbid();
            }

            if (dbUser.Email != request.CurrentEmail)
            {
                return BadRequest("Wrong current user email");
            }

            var dbUserOfNewEmail = await _userIdentityService.FindByEmailAsync(request.NewEmail);
            if (dbUserOfNewEmail != null)
            {
                return BadRequest("Choose another new email");
            }

            dbUser.Email = request.NewEmail;

            await _userIdentityService.UpdateAsync(dbUser);

            // logout user from all sessions
            var result = await _tokenService.InvalidateUserTokens(loggedInUserId, Constants.ReasonsOfRevoke.UserChangedEmail);
            if (!result)
            {
                return BadRequest("Unable to invalidate tokens");
            }

            return Ok();
        }

        [Authorize(Policy = Policies.Roles.Admin)]
        [HttpPost("ToggleUserActivation/{id}")]
        public async Task<IActionResult> ToggleUserActivation(long id)
        {

            var user = await _userIdentityService.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound("User does not exist");
            }
            user.IsActive = !user.IsActive;
            await _userIdentityService.UpdateAsync(user);

            if (!user.IsActive)
            {
                // logout user from all sessions
                var result = await _tokenService.InvalidateUserTokens(id, Constants.ReasonsOfRevoke.UserDeactivated);
                if (!result)
                {
                    return BadRequest("Unable to invalidate tokens");
                }
                return Ok(new { message = $"user: {user.Email} deactivated" });
            }
            return Ok(new { message = $"user: {user.Email} activated" });
        }
    }
}
