using AuthDemo.Cache.Interfaces;
using AuthDemo.Contracts.DataTransferObjects.Response;
using AuthDemo.Domain.Identity.Interfaces;
using AuthDemo.Infrastructure.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static AuthDemo.Cache.Constants.CacheKeys;
using Policies = AuthDemo.Security.Authorization.AuthDemoPolicies;

namespace AuthDemo.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;
        private readonly IUserIdentityService _userIdentityService;

        public UsersController(
            IMapper mapper,
            ICacheService cacheService,
            IUserIdentityService userIdentityService)
        {
            _mapper = mapper;
            _cacheService = cacheService;
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
                await _cacheService.RemoveAllResourcesPerObjectIdAsync(CacheResources.UserToken, id.ToString());
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
    }
}
