using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stadyum.API.Data;
using Stadyum.API.Models.DTOs;
using Stadyum.API.Models;


namespace Stadyum.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MatchesController : ControllerBase
    {
        private readonly StadyumDbContext _context;

        public MatchesController(StadyumDbContext context)
        {
            _context = context;
        }

        // GET: api/Matches
        // GET: api/Matches
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MatchDTO>>> GetMatches()
        {
            var matches = await _context.Matches
                .Include(m => m.Team1)
                .Include(m => m.Team2)
                .ToListAsync();

            var matchDTOs = matches.Select(m => new MatchDTO
            {
                Id = m.Id,
                Team1Id = m.Team1Id, // <<<<<<<<<< BURAYA EKLİYORUZ
                Team1Name = m.Team1?.Name ?? "Bilinmiyor",
                Team2Id = m.Team2Id, // <<<<<<<<<< BURAYA EKLİYORUZ
                Team2Name = m.Team2?.Name ?? "Bilinmiyor",
                FieldName = m.FieldName,
                MatchDate = m.MatchDate
            }).ToList();

            return Ok(matchDTOs);
        }

        // GET: api/Matches/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MatchDTO>> GetMatch(int id)
        {
            var match = await _context.Matches
                .Include(m => m.Team1)
                .Include(m => m.Team2)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (match == null)
                return NotFound();

            var matchDTO = new MatchDTO
            {
                Id = match.Id,
                Team1Id = match.Team1Id, // <<<<<<<<<< BURAYA EKLİYORUZ
                Team1Name = match.Team1?.Name ?? "Bilinmiyor",
                Team2Id = match.Team2Id, // <<<<<<<<<< BURAYA EKLİYORUZ
                Team2Name = match.Team2?.Name ?? "Bilinmiyor",
                FieldName = match.FieldName,
                MatchDate = match.MatchDate
            };

            return Ok(matchDTO);
        }


        // POST: api/Matches
        [HttpPost]
        public async Task<ActionResult<MatchDTO>> CreateMatch(MatchCreateDTO dto)
        {
            var team1 = await _context.Teams.FindAsync(dto.Team1Id);
            var team2 = await _context.Teams.FindAsync(dto.Team2Id);

            if (team1 == null || team2 == null)
                return BadRequest("Takım ID'leri geçerli değil.");

            var match = new Match
            {
                Team1Id = dto.Team1Id,
                Team2Id = dto.Team2Id,
                FieldName = dto.FieldName,
                MatchDate = DateTime.SpecifyKind(dto.MatchDate, DateTimeKind.Unspecified) // 📣 BURAYA DİKKAT
            };

            _context.Matches.Add(match);
            await _context.SaveChangesAsync();

            var result = new MatchDTO
            {
                Id = match.Id,
                Team1Name = team1.Name,
                Team2Name = team2.Name,
                FieldName = match.FieldName,
                MatchDate = match.MatchDate
            };

            return CreatedAtAction(nameof(GetMatch), new { id = match.Id }, result);
        }

        // PUT: api/Matches/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMatch(int id, MatchCreateDTO dto)
        {
            var match = await _context.Matches.FindAsync(id);
            if (match == null)
                return NotFound();

            match.Team1Id = dto.Team1Id;
            match.Team2Id = dto.Team2Id;
            match.FieldName = dto.FieldName;
            match.MatchDate = dto.MatchDate;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/Matches/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMatch(int id)
        {
            var match = await _context.Matches.FindAsync(id);
            if (match == null)
                return NotFound();

            _context.Matches.Remove(match);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
