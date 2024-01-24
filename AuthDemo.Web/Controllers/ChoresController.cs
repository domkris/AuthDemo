using AuthDemo.Contracts.DataTransferObjects.Request;
using AuthDemo.Contracts.DataTransferObjects.Response;
using AuthDemo.Infrastructure;
using AuthDemo.Infrastructure.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace AuthDemo.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ChoresController : ControllerBase
    {
        private readonly AuthDemoDbContext _context;
        private readonly IMapper _mapper;

        public ChoresController(AuthDemoDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var dbChore = await _context.Chores.ToListAsync();
            var uiChore = _mapper.Map<IEnumerable<ChoreResponse>>(dbChore);
            return Ok(uiChore);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var dbChore = await _context.Chores.FirstOrDefaultAsync(chore => chore.Id == id);

            if (dbChore == null)
            {
                return NotFound();
            }
            var uiChore = _mapper.Map<ChoreResponse>(dbChore);
            return Ok(uiChore);
        }

        [HttpPost]
        public async Task<IActionResult> Post(ChoreRequest uiChore)
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
        public async Task<IActionResult> Put(int id, ChoreRequest uiChore)
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

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
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

    }
}
