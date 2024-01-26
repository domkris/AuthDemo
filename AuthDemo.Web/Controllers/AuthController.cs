using AuthDemo.Contracts.DataTransferObjects.Request;
using AuthDemo.Contracts.DataTransferObjects.Common;
using AuthDemo.Infrastructure.Entities;
using AuthDemo.Security.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using Policies = AuthDemo.Security.Authorization.AuthDemoPolicies;

namespace AuthDemo.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly JwtSettings _jwtSettings;

        public AuthController(
            UserManager<User> userManager,
            IOptionsMonitor<JwtSettings> optionsMonitor)
        {
            _userManager = userManager;
            _jwtSettings = optionsMonitor.CurrentValue;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(AuthRegisterRequest request)
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
        [HttpPost("login")]
        public async Task<IActionResult> Login(AuthLoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByEmailAsync(request.Email);
            if(user == null)
            {
                return BadRequest("Invalid email/password");
            }

            var result = await _userManager.CheckPasswordAsync(user, request.Password);
            if(!result)
            {
                return BadRequest("Invalid email/password");
            }

            var tokenGenerator = new JwtTokenGenerator(Options.Create(_jwtSettings));
            var token = tokenGenerator.GenerateToken(user);

            return Ok(new {token});
        }

        [Authorize(Roles = Policies.Roles.Admin)]
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
            
            return Ok();
        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(AuthPasswordResetRequest request)
        {
            long.TryParse(User.FindFirstValue(ClaimTypes.Role), out long loggedInUserRole);
            long.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out long loggedInUserId);

            var dbUser = await _userManager.FindByIdAsync(request.UserId.ToString());

            if (loggedInUserRole is (long)Roles.Administrator)
            {
                if (dbUser == null)
                {
                    return BadRequest("User does not exist");
                }
                await _userManager.ChangePasswordAsync(dbUser, request.CurrentPassword, request.NewPassword);
                return Ok();
            }

            if (request.UserId != loggedInUserId)
            {
                return Forbid();
            }

            if (dbUser == null)
            {
                return BadRequest("User does not exist");
            }

            await _userManager.ChangePasswordAsync(dbUser, request.CurrentPassword, request.NewPassword);

            return Ok();
        }
    }
}
