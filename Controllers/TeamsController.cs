using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stadyum.API.Data;
using Stadyum.API.Models;
using Stadyum.API.Models.DTOs;

namespace Stadyum.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TeamsController : ControllerBase
    {
        private readonly StadyumDbContext _context;

        public TeamsController(StadyumDbContext context)
        {
            _context = context;
        }

        // 🔹 Tüm takımları getir
        [HttpGet]
        public async Task<IActionResult> GetAllTeams()
        {
            var teams = await _context.Teams
                .Include(t => t.Players)
                .ToListAsync();

            return Ok(teams);
        }

        // 🔹 Yeni takım oluştur
        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> CreateTeam([FromBody] TeamCreateDTO dto)
        {
            var team = new Team
            {
                Name = dto.Name,
                CaptainId = dto.CaptainId
            };

            _context.Teams.Add(team);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetTeamById), new { id = team.Id }, team);
        }


        // 🔹 Tekil takım getir
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTeamById(int id)
        {
            var team = await _context.Teams
                .Include(t => t.Players)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (team == null)
                return NotFound();

            return Ok(team);
        }

        // 🔹 Takımı sil
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeam(int id)
        {
            var team = await _context.Teams.FindAsync(id);
            if (team == null)
                return NotFound();

            _context.Teams.Remove(team);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
