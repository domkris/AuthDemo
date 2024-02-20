using AuthDemo.Contracts.DataTransferObjects.Request;
using AuthDemo.Contracts.DataTransferObjects.Common;
using AuthDemo.Infrastructure.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Policies = AuthDemo.Security.Authorization.AuthDemoPolicies;
using AuthDemo.Cache.Interfaces;
using AuthDemo.Security.Interfaces;
using static AuthDemo.Cache.Constants.CacheKeys;
using AuthDemo.Domain.Identity.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace AuthDemo.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AuthController : ControllerBase
    {
        private readonly ICacheService _cacheService;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IUserIdentityService _userIdentityService;

        public AuthController(
            ICacheService cacheService,
            IJwtTokenService jwtTokenService,
            IUserIdentityService userIdentityService)
        {
            _cacheService = cacheService;
            _jwtTokenService = jwtTokenService;
            _userIdentityService = userIdentityService;
        }

        [Authorize(Policy = Policies.Roles.Admin)]
        [HttpPost("CreateUser")]
        public async Task<IActionResult> CreateUser(AuthUserCreateRequest request)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingUserByEmail = await _userIdentityService.FindByEmailAsync(request.Email);
            if (existingUserByEmail != null) 
            {
                return BadRequest("User already exists");
            }
          
            var newUser = new User
            {
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                RoleId = (long)request.Role,
                UserName = await _userIdentityService.GetCustomUniqueUserName(request.FirstName, request.LastName),
            };

            var result = await _userIdentityService.CreateAsync(newUser, request.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> Login(AuthUserLoginRequest request)
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
                var token = await _jwtTokenService.GenerateToken(user!);

                return Ok(new { token });
            }

            return BadRequest("Invalid login attempt");
        }

        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            string tokenId = User.FindFirstValue(JwtRegisteredClaimNames.Jti);
            long.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out long loggedInUserId);

            // logout user from current session
            var result = await _cacheService.RemoveResourcePerObjectIdAsync(CacheResources.UserToken, tokenId, loggedInUserId.ToString());
            if(!result)
            {
                return BadRequest("Unable to logout");
            }
            return Ok(new { message = "Logout successful" });
        }

       
        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword(AuthUserPasswordChangeRequest request)
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
            await _cacheService.RemoveAllResourcesPerObjectIdAsync(CacheResources.UserToken, dbUser.Id.ToString());

            return Ok();
        }

        [HttpPost("ChangeEmail")]
        public async Task<IActionResult> ChangeEmail(AuthUserEmailChangeRequest request)
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
            await _cacheService.RemoveAllResourcesPerObjectIdAsync(CacheResources.UserToken, dbUser.Id.ToString());

            return Ok();
        }

        [HttpPost("RequestEmailChangeToken")]
        public async Task<IActionResult> RequestEmailChangeToken(AuthUserEmailChangeRequest request)
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
            await _cacheService.RemoveAllResourcesPerObjectIdAsync(CacheResources.UserToken, dbUser.Id.ToString());

            return Ok();
        }


    }
}
