using AuthDemo.Contracts.DataTransferObjects.Request;
using AuthDemo.Contracts.DataTransferObjects.Response;
using AuthDemo.Domain.Identity.Interfaces;
using AuthDemo.Infrastructure.Entities;
using AuthDemo.Security.Interfaces;
using AuthDemo.Web.Common;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Policies = AuthDemo.Security.Authorization.AuthDemoPolicies;

namespace AuthDemo.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;
        private readonly IUserIdentityService _userIdentityService;

        public UsersController(
            IMapper mapper,
            ITokenService tokenService,
            IUserIdentityService userIdentityService)
        {
            _mapper = mapper;
            _tokenService = tokenService;
            _userIdentityService = userIdentityService;
        }

        [Authorize(Policy = Policies.Roles.Admin)]
        [HttpPost("ToggleUserActivation/{id}")]
        public async Task<IActionResult> ToggleUserActivation(long id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userIdentityService.FindByIdAsync(id);
            if (user == null)
            {
                return BadRequest("User does not exist");
            }
            user.IsActive = !user.IsActive;
            await _userIdentityService.UpdateAsync(user);

            if (!user.IsActive)
            {
                // logout user from all sessions
                await _tokenService.InvalidateUserTokens(id, Constants.ReasonsOfRevoke.UserDeactivated);
            }
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var dbUsers = await _userIdentityService
                .GetAll()
                .Where(user => user.Id != 1)
                .Include(user => user.CreatedBy)
                .Include(user => user.UpdatedBy)
                .ToListAsync();

            var uiUsers = _mapper.Map<IEnumerable<UserResponse>>(dbUsers);
            return Ok(uiUsers);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(long id)
        {
            var dbUser = await _userIdentityService
                .GetAll()
                .Where(user => user.Id == id)
                .Include(user => user.CreatedBy)
                .Include(user => user.UpdatedBy)
                .FirstOrDefaultAsync();

            var uiUser = _mapper.Map<UserResponse>(dbUser);
            return Ok(uiUser);
        }

        [Authorize(Policy = Policies.Roles.Admin)]
        [HttpPost]
        public async Task<IActionResult> Post(UserPostRequest request)
        {
            if (!ModelState.IsValid)
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
    }
}
