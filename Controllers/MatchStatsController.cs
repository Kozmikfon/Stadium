using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stadyum.API.Data;
using Stadyum.API.Models;
using Stadyum.API.Models.DTOs;

namespace Stadyum.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MatchStatsController : ControllerBase
    {
        private readonly StadyumDbContext _context;

        public MatchStatsController(StadyumDbContext context)
        {
            _context = context;
        }

        // GET: api/MatchStats
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MatchStatDTO>>> GetMatchStats()
        {
            var stats = await _context.MatchStats.ToListAsync();
            return Ok(stats.Select(s => new MatchStatDTO
            {
                Id = s.Id,
                PlayerId = s.PlayerId,
                MatchId = s.MatchId,
                Goals = s.Goals,
                Assists = s.Assists,
                YellowCards = s.YellowCards,
                RedCards = s.RedCards
            }));
        }

        // GET: api/MatchStats/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MatchStatDTO>> GetMatchStat(int id)
        {
            var stat = await _context.MatchStats.FindAsync(id);
            if (stat == null) return NotFound();

            return new MatchStatDTO
            {
                Id = stat.Id,
                PlayerId = stat.PlayerId,
                MatchId = stat.MatchId,
                Goals = stat.Goals,
                Assists = stat.Assists,
                YellowCards = stat.YellowCards,
                RedCards = stat.RedCards
            };
        }

        // POST: api/MatchStats
        [HttpPost]
        public async Task<ActionResult<MatchStatDTO>> CreateMatchStat(MatchStatCreateDTO dto)
        {
            var stat = new MatchStat
            {
                PlayerId = dto.PlayerId,
                MatchId = dto.MatchId,
                Goals = dto.Goals,
                Assists = dto.Assists,
                YellowCards = dto.YellowCards,
                RedCards = dto.RedCards
            };

            _context.MatchStats.Add(stat);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMatchStat), new { id = stat.Id }, stat);
        }

        // PUT: api/MatchStats/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMatchStat(int id, MatchStatCreateDTO dto)
        {
            var stat = await _context.MatchStats.FindAsync(id);
            if (stat == null) return NotFound();

            stat.Goals = dto.Goals;
            stat.Assists = dto.Assists;
            stat.YellowCards = dto.YellowCards;
            stat.RedCards = dto.RedCards;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/MatchStats/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMatchStat(int id)
        {
            var stat = await _context.MatchStats.FindAsync(id);
            if (stat == null) return NotFound();

            _context.MatchStats.Remove(stat);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}