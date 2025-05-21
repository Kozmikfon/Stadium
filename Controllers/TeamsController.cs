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

        // 🔹 Tüm takımları getir (DTO ile)
        [HttpGet]
        public async Task<IActionResult> GetAllTeams()
        {
            var teams = await _context.Teams
                .Include(t => t.Players)
                .Include(t => t.Captain)
                    
                .Select(t => new TeamDTO
                {
                    Id = t.Id,
                    Name = t.Name,
                    CaptainId = t.Captain.Id,
                    Captain = t.Captain != null ? new PlayerDTO
                    {
                        Id = t.Captain.Id,
                        FirstName = t.Captain.FirstName,
                        LastName = t.Captain.LastName,
                        Email = t.Captain.Email,
                        Position = t.Captain.Position,
                        SkillLevel = t.Captain.SkillLevel,
                        Rating = t.Captain.Rating,
                        CreateAd = t.Captain.CreateAd,
                        TeamId = t.Players.Any(p => p.Id == t.Captain.Id) ? t.Id : null,
                        TeamName = t.Players.Any(p => p.Id == t.Captain.Id) ? t.Name : null
                    } : null,

                    Players = t.Players.Select(p => new PlayerDTO
                    {
                        Id = p.Id,
                        FirstName = p.FirstName,
                        LastName = p.LastName,
                        Email = p.Email,
                        Position = p.Position,
                        SkillLevel = p.SkillLevel,
                        Rating = p.Rating,
                        CreateAd = p.CreateAd,
                        TeamId = p.TeamId,
                        TeamName = t.Name
                    }).ToList()
                })
                .ToListAsync();

            return Ok(teams);
        }


        // 🔹 Yeni takım oluştur
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

            var captain = await _context.Players.FindAsync(dto.CaptainId);
            if (captain != null)
            {
                captain.TeamId = team.Id;
                _context.Players.Update(captain);
                await _context.SaveChangesAsync();
            }

            // 📣 Captain'ın güncellenmiş halini dahil ederek Team'i tekrar çekelim:
            var updatedTeam = await _context.Teams
                .Include(t => t.Captain)
                .FirstOrDefaultAsync(t => t.Id == team.Id);

            return CreatedAtAction(nameof(GetTeamById), new { id = team.Id }, updatedTeam);
        }


        // 🔹 Tekil takımı getir (DTO ile)
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTeamById(int id)
        {
            var team = await _context.Teams
                .Include(t => t.Players)
                .Include(t => t.Captain)
                .Select(t => new TeamDTO
                {
                    Id = t.Id,
                    Name = t.Name,
                    CaptainId = t.Captain.Id,
                    Captain = t.Captain != null ? new PlayerDTO
                    {
                        Id = t.Captain.Id,
                        FirstName = t.Captain.FirstName,
                        LastName = t.Captain.LastName,
                        Email = t.Captain.Email,
                        Position = t.Captain.Position,
                        SkillLevel = t.Captain.SkillLevel,
                        Rating = t.Captain.Rating,
                        CreateAd = t.Captain.CreateAd,
                        TeamId = t.Players.Any(p => p.Id == t.Captain.Id) ? t.Id : null,
                        TeamName = t.Players.Any(p => p.Id == t.Captain.Id) ? t.Name : null
                    } : null,

                    Players = t.Players.Select(p => new PlayerDTO
                    {
                        Id = p.Id,
                        FirstName = p.FirstName,
                        LastName = p.LastName,
                        Email = p.Email,
                        Position = p.Position,
                        SkillLevel = p.SkillLevel,
                        Rating = p.Rating,
                        CreateAd = p.CreateAd,
                        TeamId = p.TeamId,
                        TeamName = t.Name
                    }).ToList()
                })
                .FirstOrDefaultAsync(t => t.Id == id);

            if (team == null)
                return NotFound();

            return Ok(team);
        }


        //takım ozelligi
        [HttpGet("profile/{playerId}")]
        public async Task<IActionResult> GetTeamProfile(int playerId)
        {
            var player = await _context.Players
                .Include(p => p.Team)
                .ThenInclude(t => t.Captain)
                .FirstOrDefaultAsync(p => p.Id == playerId);

            if (player?.Team == null)
                return NotFound("Oyuncu herhangi bir takıma ait değil.");

            var team = player.Team;

            int totalMatches = await _context.Matches
                .CountAsync(m => m.Team1Id == team.Id || m.Team2Id == team.Id);

            int memberCount = await _context.TeamMembers
                .CountAsync(m => m.TeamId == team.Id);

            return Ok(new
            {
                team.Id,
                team.Name,
                CaptainId = team.CaptainId,
                CaptainName = team.Captain.FirstName + " " + team.Captain.LastName,
                MemberCount = memberCount,
                TotalMatches = totalMatches
            });
        }


        //takım karsilastirma
        [HttpGet("compare/{team1Id}/{team2Id}")]
        public async Task<IActionResult> CompareTeams(int team1Id, int team2Id)
        {
            var team1 = await _context.Teams.FindAsync(team1Id);
            var team2 = await _context.Teams.FindAsync(team2Id);

            if (team1 == null || team2 == null)
                return NotFound("Takımlardan biri bulunamadı.");

            var result = new List<TeamCompareDTO>();

            foreach (var team in new[] { team1, team2 })
            {
                int totalMatches = await _context.Matches
                    .CountAsync(m => m.Team1Id == team.Id || m.Team2Id == team.Id);

                double avgRating = await _context.Players
                    .Where(p => p.TeamId == team.Id && p.Rating > 0)
                    .AverageAsync(p => (double?)p.Rating) ?? 0;

                result.Add(new TeamCompareDTO
                {
                    TeamName = team.Name,
                    TotalMatches = totalMatches,
                    AverageRating = Math.Round(avgRating, 1)
                });
            }

            return Ok(result);
        }



        // takım detayı
        [HttpGet("details/{teamId}")]
        public async Task<IActionResult> GetTeamDetails(int teamId)
        {
            var team = await _context.Teams
                .Include(t => t.Captain)
                .FirstOrDefaultAsync(t => t.Id == teamId);

            if (team == null)
                return NotFound("Takım bulunamadı.");

            int memberCount = await _context.TeamMembers
                .CountAsync(m => m.TeamId == team.Id);

            int matchCount = await _context.Matches
                .CountAsync(m => m.Team1Id == team.Id || m.Team2Id == team.Id);

            return Ok(new
            {
                team.Name,
                CaptainName = team.Captain.FirstName + " " + team.Captain.LastName,
                MemberCount = memberCount,
                TotalMatches = matchCount
            });
        }
        // kaptanı degistir

     




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

        // kaptan seçme
        [HttpPut("assign-captain")]
        public async Task<IActionResult> AssignCaptain(int teamId, int newCaptainId)
        {
            var team = await _context.Teams.FindAsync(teamId);
            if (team == null) return NotFound();

            team.CaptainId = newCaptainId;
            await _context.SaveChangesAsync();

            return Ok();
        }

        //turnuva
        // GET: api/Teams/tournament-teams
        [HttpGet("tournament-teams")]
        public async Task<IActionResult> GetTournamentTeams()
        {
            var tournamentTeams = await _context.Teams
                .Where(t => t.IsInTournament == true)
                .ToListAsync();

            return Ok(tournamentTeams);
        }

        //turnuva takım olusturma
        [HttpPost("create-tournament-team")]
        public async Task<IActionResult> CreateTournamentTeam([FromBody] TeamCreateDTO dto)
        {
            var team = new Team
            {
                Name = dto.Name,
                CaptainId = dto.CaptainId,
                IsInTournament = true
            };

            _context.Teams.Add(team);
            await _context.SaveChangesAsync();

            return Ok(team);
        }

        //turnuva takım ekleme
        [HttpPut("add-to-tournament/{teamId}")]
        public async Task<IActionResult> AddTeamToTournament(int teamId)
        {
            var team = await _context.Teams.FindAsync(teamId);
            if (team == null) return NotFound("Takım bulunamadı.");

            team.IsInTournament = true;
            await _context.SaveChangesAsync();

            return Ok("Takım turnuvaya eklendi.");
        }



        // 🔹 Takımı güncelle
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTeam(int id, [FromBody] TeamUpdateDTO dto)
        {
            var team = await _context.Teams.FindAsync(id);
            if (team == null)
                return NotFound();

            // 🔹 İsim güncellemesi (gönderildiyse)
            if (!string.IsNullOrWhiteSpace(dto.Name))
            {
                team.Name = dto.Name;
            }

            // 🔹 Kaptan güncellemesi (gönderildiyse)
            if (dto.CaptainId.HasValue)
            {
                var captain = await _context.Players.FindAsync(dto.CaptainId.Value);
                if (captain == null)
                    return BadRequest("Belirtilen kaptan bulunamadı.");

                team.CaptainId = dto.CaptainId.Value;

                // 🧠 Burada kaptanı takıma bağlayalım:
                if (captain.TeamId != team.Id)
                    return BadRequest("Kaptan, bu takıma ait değil.");


                // Bu satırı ekleyerek EF'nin değişikliği izlemesini sağlıyoruz:
                _context.Players.Update(captain);
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Teams.Any(t => t.Id == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }



    }
}
