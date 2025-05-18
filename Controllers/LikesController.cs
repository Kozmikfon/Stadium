using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stadyum.API.Data;
using Stadyum.API.Models;
using Stadyum.API.Models.DTOs;

namespace Stadyum.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LikesController : ControllerBase
    {
        private readonly StadyumDbContext _context;

        public LikesController(StadyumDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> ToggleLike([FromBody] LikeDTO dto)
        {
            var existing = await _context.Likes
                .FirstOrDefaultAsync(l => l.MatchId == dto.MatchId && l.PlayerId == dto.PlayerId);

            if (existing != null)
            {
                _context.Likes.Remove(existing);
                await _context.SaveChangesAsync();
                return Ok(new { liked = false });
            }

            var like = new Like
            {
                PlayerId = dto.PlayerId,
                MatchId = dto.MatchId
            };

            _context.Likes.Add(like);
            await _context.SaveChangesAsync();

            return Ok(new { liked = true });
        }

        [HttpGet("count/{matchId}")]
        public async Task<IActionResult> GetLikeCount(int matchId)
        {
            var count = await _context.Likes.CountAsync(l => l.MatchId == matchId);
            return Ok(count);
        }

        [HttpGet("is-liked")]
        public async Task<IActionResult> IsLiked([FromQuery] int matchId, [FromQuery] int playerId)
        {
            var liked = await _context.Likes.AnyAsync(l => l.MatchId == matchId && l.PlayerId == playerId);
            return Ok(liked);
        }
    }
}
