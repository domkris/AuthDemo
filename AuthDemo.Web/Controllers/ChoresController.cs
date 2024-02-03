using AuthDemo.Contracts.DataTransferObjects.Request;
using AuthDemo.Contracts.DataTransferObjects.Response;
using AuthDemo.Contracts.DataTransferObjects.Common;
using AuthDemo.Infrastructure;
using AuthDemo.Infrastructure.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Policies = AuthDemo.Security.Authorization.AuthDemoPolicies;
using AuthDemo.Domain;
using AuthDemo.Domain.Repositories.Interfaces;
using AuthDemo.Domain.Identity.Interfaces;

namespace AuthDemo.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ChoresController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserIdentityService _userIdentityService;

        public ChoresController(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            IUserIdentityService userIdentityService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _userIdentityService = userIdentityService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var dbChore = await _unitOfWork.Chores
                .GetAll()
                .Include(chore => chore.CreatedBy)
                .Include(chore => chore.UpdatedBy)
                .Include(chore => chore.UserAssignee)
                .ToListAsync();
           
            var uiChore = _mapper.Map<IEnumerable<ChoreResponse>>(dbChore);
            return Ok(uiChore);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(long id)
        {
            
            var dbChore = await _unitOfWork.Chores
                .GetAll()
                .Include(chore => chore.CreatedBy)
                .Include(chore => chore.UpdatedBy)
                .Include(chore => chore.UserAssignee)
                .FirstOrDefaultAsync(chore => chore.Id == id);
            
            if (dbChore == null)
            {
                return NotFound();
            }

            var uiChore = _mapper.Map<ChoreResponse>(dbChore);
            return Ok(uiChore);
        }

        [Authorize(Policy = Policies.Roles.AdminOrManager)]
        [HttpPost]
        public async Task<IActionResult> Post(ChoreEditRequest uiChore)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Chore dbChore = new()
            {
                Title = uiChore.Title,
                Description = uiChore.Description
            };

            _unitOfWork.Chores.Add(dbChore);
            await _unitOfWork.SaveAsync();

            return CreatedAtAction("Get", new { dbChore.Id });
        }

        [Authorize(Policy = Policies.Roles.AdminOrManager)]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(long id, ChoreEditRequest uiChore)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var dbChore = await _unitOfWork.Chores
                .GetAll()
                .FirstOrDefaultAsync(chore => chore.Id == id);

            if (dbChore == null)
            {
                return NotFound();
            }

            dbChore.Title = uiChore.Title;
            dbChore.Description = uiChore.Description;

            await _unitOfWork.SaveAsync();

            return Ok();
        }

        [Authorize(Policy = Policies.Roles.Admin)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var dbChore = await _unitOfWork.Chores
                .GetAll()
                .FirstOrDefaultAsync(chore => chore.Id == id);

            if (dbChore == null)
            {
                return NotFound();
            }
            _unitOfWork.Chores.Remove(dbChore);
            await _unitOfWork.SaveAsync();

            return Ok();
        }

        [Authorize(Policy = Policies.Roles.AdminOrManager)]
        [HttpPut("AssignUser")]
        public async Task<IActionResult> AssignUser(ChoreAssignUserRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var dbUser = await _userIdentityService.FindByIdAsync(request.UserId);
            if (dbUser == null)
            {
                return BadRequest("User does not exists");
            }

            var dbChore = await _unitOfWork.Chores
                .GetAll()
                .FirstOrDefaultAsync(chore => chore.Id == request.ChoreId);

            if (dbChore == null)
            {
                return BadRequest("Chore does not exists");
            }

            dbChore.UserAssigneeId = request.UserId;
            await _unitOfWork.SaveAsync();

            return Ok();
        }

        [HttpPut("Finish/{id}")]
        public async Task<IActionResult> Finish(long id)
        {
            long.TryParse(User.FindFirstValue(ClaimTypes.Role), out long userRole);
            long.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out long userId);

            var dbChore = await _unitOfWork.Chores
                .GetAll()
                .FirstOrDefaultAsync(chore => chore.Id == id);

            if (dbChore == null)
            {
                return BadRequest("Chore does not exists");
            }

            if (userRole is not (long)Roles.Employee)
            {
                dbChore.IsFinished = !dbChore.IsFinished;
            }
            else
            {
                if (dbChore.UserAssigneeId != userId)
                {
                    return Forbid();
                }

                dbChore.IsFinished = !dbChore.IsFinished;
            }

            await _unitOfWork.SaveAsync();

            return Ok();
        }

        [Authorize(Policy = Policies.Roles.AdminOrManager)]
        [HttpPut("Approve/{id}")]
        public async Task<IActionResult> Approve(long id)
        {
            var dbChore = await _unitOfWork.Chores
                .GetAll()
                .FirstOrDefaultAsync(chore => chore.Id == id);

            if (dbChore == null)
            {
                return BadRequest("Chore does not exists");
            }

            dbChore.IsApproved = !dbChore.IsApproved;

            await _unitOfWork.SaveAsync();

            return Ok();
        }
    }
}
