using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stadyum.API.Data;
using Stadyum.API.Models;

namespace Stadyum.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly StadyumDbContext _context;

        public AdminController(StadyumDbContext context)
        {
            _context = context;
        }

        // Tüm oyuncuları listele
        [HttpGet("players")]
        public async Task<IActionResult> GetAllPlayers()
        {
            var players = await _context.Players.Include(p => p.Team).ToListAsync();
            return Ok(players);
        }

        // Tüm takımları listele
        [HttpGet("teams")]
        public async Task<IActionResult> GetAllTeams()
        {
            var teams = await _context.Teams.Include(t => t.Players).ToListAsync();
            return Ok(teams);
        }

        // Belirli bir maçı sil
        [HttpDelete("match/{id}")]
        public async Task<IActionResult> DeleteMatch(int id)
        {
            var match = await _context.Matches.FindAsync(id);
            if (match == null)
                return NotFound("Maç bulunamadı");

            _context.Matches.Remove(match);
            await _context.SaveChangesAsync();
            return Ok("Maç silindi");
        }

        // Oyuncuyu admin yap / adminlikten çıkar
        [HttpPut("toggle-admin/{playerId}")]
        public async Task<IActionResult> ToggleAdmin(int playerId)
        {
            var player = await _context.Players.FindAsync(playerId);
            if (player == null)
                return NotFound("Oyuncu bulunamadı");

            player.IsAdmin = !player.IsAdmin;
            await _context.SaveChangesAsync();
            return Ok(new { player.Id, player.Email, player.IsAdmin });
        }

        // Tüm yorumları getir
        [HttpGet("reviews")]
        public async Task<IActionResult> GetAllReviews()
        {
            var reviews = await _context.Reviews.Include(r => r.Match).ToListAsync();
            return Ok(reviews);
        }
    }
}
