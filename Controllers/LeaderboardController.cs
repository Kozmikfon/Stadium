using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stadyum.API.Data;
using Stadyum.API.Models;
using Stadyum.API.Models.DTOs;

namespace Stadyum.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaderboardController : ControllerBase
    {
        private readonly StadyumDbContext _context;

        public LeaderboardController(StadyumDbContext context)
        {
            _context = context;
        }

        // 1. En yüksek puanlı oyuncular
        [HttpGet("top-players")]
        public async Task<IActionResult> GetTopPlayers()
        {
            var topPlayers = await _context.Players
                .OrderByDescending(p => p.Rating)
                .Take(10)
                .Select(p => new LeaderboardPlayerDTO
                {
                    Id = p.Id,
                    FullName = p.FirstName + " " + p.LastName,
                    Rating = p.Rating,
                    MatchCount = _context.Matches.Count(m => m.Team1Id == p.TeamId || m.Team2Id == p.TeamId)
                })
                .ToListAsync();

            return Ok(topPlayers);
        }

        // 2. En çok maç yapan oyuncular
        [HttpGet("top-match-players")]
        public async Task<IActionResult> GetTopMatchPlayers()
        {
            var matchCounts = await _context.Players
                .Select(p => new
                {
                    p.Id,
                    FullName = p.FirstName + " " + p.LastName,
                    Rating = p.Rating,
                    MatchCount = _context.Matches.Count(m => m.Team1Id == p.TeamId || m.Team2Id == p.TeamId)
                })
                .OrderByDescending(p => p.MatchCount)
                .Take(10)
                .Select(p => new LeaderboardPlayerDTO
                {
                    Id = p.Id,
                    FullName = p.FullName,
                    Rating = p.Rating,
                    MatchCount = p.MatchCount
                })
                .ToListAsync();

            return Ok(matchCounts);
        }

        // 3. En çok galibiyet alan takımlar
        [HttpGet("top-teams")]
        public async Task<IActionResult> GetTopTeams()
        {
            var teams = await _context.Teams
                .Select(t => new LeaderboardTeamDTO
                {
                    Id = t.Id,
                    Name = t.Name,
                    MatchCount = _context.Matches.Count(m => m.Team1Id == t.Id || m.Team2Id == t.Id)
                })
                .OrderByDescending(t => t.MatchCount)
                .Take(10)
                .ToListAsync();

            return Ok(teams);
        }

        //oyuncu karsilastirma


    }
}
