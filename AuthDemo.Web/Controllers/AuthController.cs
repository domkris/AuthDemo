using AuthDemo.Contracts.DataTransferObjects.Request;
using AuthDemo.Infrastructure.Entities;
using AuthDemo.Security.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AuthDemo.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
                UserName = request.UserName
            };

            var result = await _userManager.CreateAsync(newUser, request.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok();
        }

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
    }
}
