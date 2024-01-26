using AuthDemo.Contracts.DataTransferObjects.Request;
using AuthDemo.Contracts.DataTransferObjects.Response;
using AuthDemo.Infrastructure;
using AuthDemo.Infrastructure.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Policies = AuthDemo.Security.Authorization.AuthDemoPolicies;

namespace AuthDemo.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ChoresController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly AuthDemoDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IAuthorizationService _authorizationService;


        public ChoresController(
            IMapper mapper,
            AuthDemoDbContext context,
            UserManager<User> userManager,
            IAuthorizationService authorizationService)
        {
            _mapper = mapper;
            _context = context;
            _userManager = userManager;
            _authorizationService = authorizationService;


        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _authorizationService.AuthorizeAsync(User, Policies.Roles.AdminAndManager);
            var dbChore = await _context.Chores
                .Include(chore => chore.CreatedBy)
                .Include(chore => chore.UserAssignee)
                .ToListAsync();

            var uiChore = _mapper.Map<IEnumerable<ChoreResponse>>(dbChore);
            return Ok(uiChore);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(long id)
        {
            var dbChore = await _context.Chores
                .Include(chore => chore.CreatedBy)
                .Include(chore => chore.UserAssignee)
                .FirstOrDefaultAsync(chore => chore.Id == id);

            if (dbChore == null)
            {
                return NotFound();
            }
            var uiChore = _mapper.Map<ChoreResponse>(dbChore);
            return Ok(uiChore);
        }

        [HttpPost]
        [Authorize(Roles = Policies.Roles.AdminAndManager)]
        public async Task<IActionResult> Post(ChoreEditRequest uiChore)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Chore dbChore = new() { 
                Title = uiChore.Title, 
                Description = uiChore.Description 
            };
            await _context.Chores.AddAsync(dbChore);
            await _context.SaveChangesAsync();
            return CreatedAtAction("Get", new { dbChore.Id});
        }

        [HttpPut("{id}")]
        [Authorize(Roles = Policies.Roles.AdminAndManager)]
        public async Task<IActionResult> Put(long id, ChoreEditRequest uiChore)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var dbChore = await _context.Chores.FirstOrDefaultAsync(x => x.Id == id);

            if (dbChore == null)
            {
                return NotFound();
            }

            dbChore.Title = uiChore.Title;
            dbChore.Description = uiChore.Description;

            await _context.SaveChangesAsync();

            return Ok();
        }

        [Authorize(Roles = Policies.Roles.Admin)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var dbChore = await _context.Chores.FirstOrDefaultAsync(x => x.Id == id);

            if (dbChore == null)
            {
                return NotFound();
            }

            _context.Chores.Remove(dbChore);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("AssignUser")]
        [Authorize(Roles = Policies.Roles.AdminAndManager)]
        public async Task<IActionResult> AssignUser(ChoreAssignUserRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingUser = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (existingUser == null)
            {
                return BadRequest("User does not exists");
            }

            var dbChore = await _context.Chores.FirstOrDefaultAsync(chore => chore.Id == request.ChoreId);

            if (dbChore == null)
            {
                return BadRequest("Chore does not exists");
            }

            dbChore.UserAssigneeId = request.UserId;
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("Finish/{id}")]
        public async Task<IActionResult> Finish(long id)
        {
            
            var dbChore = await _context.Chores
                .Include(chore => chore.UserAssignee)
                .FirstOrDefaultAsync(chore => chore.Id == id);

            if (dbChore == null)
            {
                return BadRequest("Chore does not exists");
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

         

            return Ok();
        }

    }
}
