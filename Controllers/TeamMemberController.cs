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

        //takımdan ayrıl
        [HttpDelete("leave/{playerId}")]
        public async Task<IActionResult> LeaveTeam(int playerId)
        {
            var membership = await _context.TeamMembers.FirstOrDefaultAsync(m => m.PlayerId == playerId);
            if (membership == null)
                return NotFound("Üyelik bulunamadı.");

            _context.TeamMembers.Remove(membership);

            var player = await _context.Players.FindAsync(playerId);
            if (player != null)
                player.TeamId = null;

            await _context.SaveChangesAsync();
            return Ok("Takımdan ayrılındı.");
        }



        // DELETE: api/TeamMembers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeamMember(int id)
        {
            var teamMember = await _context.TeamMembers.FindAsync(id);

            if (teamMember == null)
                return NotFound();

            _context.TeamMembers.Remove(teamMember);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
