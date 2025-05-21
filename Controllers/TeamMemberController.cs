using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stadyum.API.Data;
using Stadyum.API.Models;
using Stadyum.API.Models.DTOs;

namespace Stadyum.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamMembersController : ControllerBase
    {
        private readonly StadyumDbContext _context;

        public TeamMembersController(StadyumDbContext context)
        {
            _context = context;
        }

        // GET: api/TeamMembers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TeamMemberDTO>>> GetTeamMembers()
        {
            var teamMembers = await _context.TeamMembers.ToListAsync();

            var teamMemberDTOs = teamMembers.Select(tm => new TeamMemberDTO
            {
                Id = tm.Id,
                TeamId = tm.TeamId,
                PlayerId = tm.PlayerId
            }).ToList();

            return Ok(teamMemberDTOs);
        }

        // GET: api/TeamMembers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TeamMemberDTO>> GetTeamMember(int id)
        {
            var teamMember = await _context.TeamMembers.FindAsync(id);

            if (teamMember == null)
                return NotFound();

            var teamMemberDTO = new TeamMemberDTO
            {
                Id = teamMember.Id,
                TeamId = teamMember.TeamId,
                PlayerId = teamMember.PlayerId
            };

            return Ok(teamMemberDTO);
        }

        // POST: api/TeamMembers
        [HttpPost]
        public async Task<ActionResult<TeamMemberDTO>> CreateTeamMember(TeamMemberCreateDTO dto)
        {
            // ✅ Önce zaten kayıtlı mı kontrol et
            bool alreadyExists = await _context.TeamMembers
                .AnyAsync(t => t.TeamId == dto.TeamId && t.PlayerId == dto.PlayerId);

            if (alreadyExists)
            {
                return BadRequest("Oyuncu zaten bu takıma kayıtlı.");
            }
            var teamMember = new TeamMember
            {
                TeamId = dto.TeamId,
                PlayerId = dto.PlayerId
            };

            var player = await _context.Players.FindAsync(dto.PlayerId);
            if (player != null)
            {
                player.TeamId = dto.TeamId;
                // update player
                _context.Players.Update(player);
            }


            _context.TeamMembers.Add(teamMember);
            await _context.SaveChangesAsync();

            var result = new TeamMemberDTO
            {
                Id = teamMember.Id,
                TeamId = teamMember.TeamId,
                PlayerId = teamMember.PlayerId
            };

            return CreatedAtAction(nameof(GetTeamMember), new { id = teamMember.Id }, result);
        }

        // ✅ Takımdan ayrılma endpoint'i
        [HttpDelete("leave/{playerId}")]
        public async Task<IActionResult> LeaveTeam(int playerId)
        {
            var membership = await _context.TeamMembers.FirstOrDefaultAsync(m => m.PlayerId == playerId);
            var player = await _context.Players.FindAsync(playerId);

            if (membership == null && player?.TeamId == null)
                return NotFound("Oyuncunun bir takım üyeliği bulunamadı.");

            // 👇 Kaptan kontrolü
            if (player?.TeamId != null)
            {
                var team = await _context.Teams
                    .Include(t => t.Players)
                    .FirstOrDefaultAsync(t => t.Id == player.TeamId);

                if (team != null && team.CaptainId == playerId)
                {
                    var otherPlayers = team.Players.Where(p => p.Id != playerId).ToList();
                    if (otherPlayers.Count > 0)
                    {
                        return BadRequest("⚠️ Kaptan takımdan ayrılamaz. Önce yeni bir kaptan atamalısınız.");
                    }

                    // Eğer kaptan ve takımda tek kişi kaldıysa, takımı silebiliriz (isteğe bağlı)
                    // _context.Teams.Remove(team); // ← yorumda bırakıyorum
                }
            }

            if (membership != null)
                _context.TeamMembers.Remove(membership);

            if (player != null)
            {
                player.TeamId = null;
                _context.Players.Update(player);
            }

            await _context.SaveChangesAsync();
            return Ok("✅ Takımdan başarıyla ayrıldınız.");
        }








        // DELETE: api/TeamMembers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeamMember(int id)
        {
            var teamMember = await _context.TeamMembers.FindAsync(id);
            if (teamMember == null)
                return NotFound();

            // 🎯 İlgili oyuncuyu da bul
            var player = await _context.Players.FindAsync(teamMember.PlayerId);
            if (player != null)
            {
                player.TeamId = null; // 🔥 Takım ilişkisini kaldır
                _context.Players.Update(player);
            }

            _context.TeamMembers.Remove(teamMember);
            await _context.SaveChangesAsync();

            return Ok("Takımdan ayrılındı.");
        }

    }
}
