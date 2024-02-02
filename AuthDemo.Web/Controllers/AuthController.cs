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

namespace AuthDemo.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AuthController : ControllerBase
    {
        private readonly ICacheService _cacheService;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IJwtTokenService _jwtTokenService;

        public AuthController(
            ICacheService cacheService,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IJwtTokenService jwtTokenService)
        {
            _cacheService = cacheService;
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtTokenService = jwtTokenService;
        }

        [Authorize(Policy = Policies.Roles.Admin)]
        [HttpPost("CreateUser")]
        public async Task<IActionResult> CreateUser(AuthCreateUserRequest request)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if(existingUser != null) 
            {
                return BadRequest("User already exists");
            }

            var newUser = new User
            {
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                RoleId = (long)request.Role,
                UserName = string.Concat(request.FirstName, request.LastName).ToLower(),
            };

            var result = await _userManager.CreateAsync(newUser, request.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok();
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

            var user = await _userManager.FindByEmailAsync(request.Email);
            if(user == null) 
            {                 
                return BadRequest("Invalid login attempt");       
            }

            var result = await _signInManager.PasswordSignInAsync(user.UserName!, request.Password, rememberMe, lockoutOnFailure);

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

       
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(AuthPasswordResetRequest request)
        {
            long.TryParse(User.FindFirstValue(ClaimTypes.Role), out long loggedInUserRole);
            long.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out long loggedInUserId);

            var dbUser = await _userManager.FindByIdAsync(request.UserId.ToString());

            if (request.UserId != loggedInUserId && loggedInUserRole is not (long)Roles.Administrator)
            {
                return Forbid();
            }

            if (dbUser == null)
            {
                return BadRequest("User does not exist");
            }

            await _userManager.ChangePasswordAsync(dbUser, request.CurrentPassword, request.NewPassword);

            // logout user from all sessions
            await _cacheService.RemoveAllResourcesPerObjectIdAsync(CacheResources.UserToken, dbUser.Id.ToString());

            return Ok();
        }
    }
}
