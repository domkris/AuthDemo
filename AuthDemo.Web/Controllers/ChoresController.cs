using AuthDemo.Infrastructure;
using AuthDemo.Infrastructure.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthDemo.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChoresController : ControllerBase
    {
        private readonly AuthDemoDbContext _context;

        public ChoresController(AuthDemoDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _context.Chores.ToListAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _context.Chores.FirstOrDefaultAsync(x => x.Id == id);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Post(Chore chore)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _context.Chores.AddAsync(chore);
            await _context.SaveChangesAsync();
            return Created();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, Chore uiChore)
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
