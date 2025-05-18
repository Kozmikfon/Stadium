using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stadyum.API.Data;
using Stadyum.API.Models;
using Stadyum.API.Models.DTOs;

namespace Stadyum.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendanceController : ControllerBase
    {
        private readonly StadyumDbContext _context;

        public AttendanceController(StadyumDbContext context)
        {
            _context = context;
        }

        // GET: api/Attendance/match/1
        [HttpGet("match/{matchId}")]
        public async Task<ActionResult<IEnumerable<AttendanceDTO>>> GetAttendanceByMatch(int matchId)
        {
            var records = await _context.Attendances
                .Include(a => a.Player)
                .Where(a => a.MatchId == matchId)
                .Select(a => new AttendanceDTO
                {
                    Id = a.Id,
                    PlayerId = a.PlayerId,
                    MatchId = a.MatchId,
                    IsPresent = a.IsPresent,
                    CheckedAt = a.CheckedAt
                })
                .ToListAsync();

            return Ok(records);
        }

        // GET: api/Attendance/player/5
        [HttpGet("player/{playerId}")]
        public async Task<ActionResult<IEnumerable<AttendanceDTO>>> GetAttendanceByPlayer(int playerId)
        {
            var records = await _context.Attendances
                .Include(a => a.Match)
                .Where(a => a.PlayerId == playerId)
                .Select(a => new AttendanceDTO
                {
                    Id = a.Id,
                    PlayerId = a.PlayerId,
                    MatchId = a.MatchId,
                    IsPresent = a.IsPresent,
                    CheckedAt = a.CheckedAt
                })
                .ToListAsync();

            return Ok(records);
        }

        // POST: api/Attendance
        [HttpPost]
        public async Task<ActionResult> MarkAttendance([FromBody] AttendanceCreateDTO dto)
        {
            var exists = await _context.Attendances
                .AnyAsync(a => a.PlayerId == dto.PlayerId && a.MatchId == dto.MatchId);

            if (exists)
            {
                return BadRequest("Oyuncu zaten bu maç için kaydedilmiş.");
            }

            var attendance = new Attendance
            {
                PlayerId = dto.PlayerId,
                MatchId = dto.MatchId,
                IsPresent = dto.IsPresent,
                CheckedAt = DateTime.UtcNow
            };

            _context.Attendances.Add(attendance);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Katılım kaydedildi" });
        }

        // PUT: api/Attendance/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAttendance(int id, [FromBody] AttendanceUpdateDTO dto)
        {
            var attendance = await _context.Attendances.FindAsync(id);
            if (attendance == null)
                return NotFound();

            attendance.IsPresent = dto.IsPresent;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Katılım durumu güncellendi." });
        }

        // DELETE: api/Attendance/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAttendance(int id)
        {
            var attendance = await _context.Attendances.FindAsync(id);
            if (attendance == null)
                return NotFound();

            _context.Attendances.Remove(attendance);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Katılım kaydı silindi." });
        }
    }
}
