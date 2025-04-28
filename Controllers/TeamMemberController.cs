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
            var teamMember = new TeamMember
            {
                TeamId = dto.TeamId,
                PlayerId = dto.PlayerId
            };

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
