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
                MatchDate = match.MatchDate,

                // ✅ Eklenen yer
                Team1CaptainId = match.Team1?.CaptainId ?? 0,
                Team2CaptainId = match.Team2?.CaptainId ?? 0
            };

            return Ok(matchDTO);
        }

        // maçtaki oyunuc sayisi
        // GET: api/Matches/with-accepted-count
        [HttpGet("with-accepted-count")]
        public async Task<IActionResult> GetMatchesWithAcceptedCount()
        {
            var matches = await _context.Matches
                .Include(m => m.Team1)
                .Include(m => m.Team2)
                .Select(m => new
                {
                    m.Id,
                    m.FieldName,
                    m.MatchDate,
                    Team1Name = m.Team1 != null ? m.Team1.Name : "Bilinmiyor",
                    Team2Name = m.Team2 != null ? m.Team2.Name : "Bilinmiyor",
                    AcceptedCount = _context.Offers.Count(o => o.MatchId == m.Id && o.Status == "Accepted")
                })
                .ToListAsync();

            return Ok(matches);
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




        [HttpGet("calendar")]
        public async Task<IActionResult> GetAllMatchesForCalendar()
        {
            var matches = await _context.Matches
                .Include(m => m.Team1)
                .Include(m => m.Team2)
                .Select(m => new
                {
                    m.Id,
                    m.MatchDate,
                    m.FieldName,
                    Team1Name = m.Team1.Name,
                    Team2Name = m.Team2.Name
                })
                .ToListAsync();

            return Ok(matches);
        }

        // MatchesController.cs
        [HttpGet("byPlayer/{playerId}")]
        public async Task<IActionResult> GetMatchesByPlayer(int playerId)
        {
            var player = await _context.Players
                .Include(p => p.Team)
                .FirstOrDefaultAsync(p => p.Id == playerId);

            if (player?.Team == null)
                return NotFound("Oyuncunun takımı yok.");

            var matches = await _context.Matches
                .Where(m => m.Team1Id == player.Team.Id || m.Team2Id == player.Team.Id)
                .ToListAsync();

            return Ok(matches);
        }



        //web için
        [HttpGet("upcoming")]
        public async Task<IActionResult> GetUpcomingMatches()
        {
            var upcomingMatches = await _context.Matches
                .Include(m => m.Team1)
                .Include(m => m.Team2)
                .Where(m => m.MatchDate >= DateTime.Now)
                .OrderBy(m => m.MatchDate)
                .Take(5)
                .Select(m => new
                {
                    m.MatchDate,
                    Team1Name = m.Team1.Name,
                    Team2Name = m.Team2.Name
                })
                .ToListAsync();

            return Ok(upcomingMatches);
        }

        // turnuva macları
        [HttpGet("tournament-matches")]
        public async Task<IActionResult> GetTournamentMatches()
        {
            var tournamentTeamIds = await _context.Teams
                .Where(t => t.IsInTournament == true)
                .Select(t => t.Id)
                .ToListAsync();

            var matches = await _context.Matches
                .Include(m => m.Team1)
                .Include(m => m.Team2)
                .Where(m => tournamentTeamIds.Contains(m.Team1Id) && tournamentTeamIds.Contains(m.Team2Id))
                .ToListAsync();

            return Ok(matches);
        }
        //turnuva macı olusturma
        [HttpPost("create-tournament-match")]
        public async Task<IActionResult> CreateTournamentMatch([FromBody] MatchCreateDTO dto)
        {
            var team1 = await _context.Teams.FindAsync(dto.Team1Id);
            var team2 = await _context.Teams.FindAsync(dto.Team2Id);

            if (team1 == null || team2 == null)
                return BadRequest("Takımlar bulunamadı.");
            if (!team1.IsInTournament || !team2.IsInTournament)
                return BadRequest("Her iki takım da turnuvada olmalı.");

            var match = new Match
            {
                Team1Id = dto.Team1Id,
                Team2Id = dto.Team2Id,
                MatchDate = dto.MatchDate,
                FieldName = dto.FieldName
            };

            _context.Matches.Add(match);
            await _context.SaveChangesAsync();

            return Ok(match);
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
