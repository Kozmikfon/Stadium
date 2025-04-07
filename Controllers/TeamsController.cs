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

            return CreatedAtAction(nameof(GetTeamById), new { id = team.Id }, team);
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
                var captainExists = await _context.Players.AnyAsync(p => p.Id == dto.CaptainId.Value);
                if (!captainExists)
                    return BadRequest("Belirtilen kaptan bulunamadı.");

                team.CaptainId = dto.CaptainId.Value;
                Console.WriteLine($"Yeni isim: {team.Name}, Yeni kaptan ID: {team.CaptainId}");

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
