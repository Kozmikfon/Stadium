using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stadyum.API.Data;
using Stadyum.API.Models;
using Stadyum.API.Models.DTOs;

namespace Stadyum.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentLikesController : ControllerBase
    {
        private readonly StadyumDbContext _context;

        public CommentLikesController(StadyumDbContext context)
        {
            _context = context;
        }

        // Beğeni ekle/kaldır
        [HttpPost]
        public async Task<IActionResult> ToggleLike([FromBody] CommentLikeDTO dto)
        {
            if (dto.ReviewId <= 0 || dto.PlayerId <= 0)
                return BadRequest("Eksik veya hatalı veri.");

            var existing = await _context.CommentLikes
                .FirstOrDefaultAsync(cl => cl.ReviewId == dto.ReviewId && cl.PlayerId == dto.PlayerId);

            if (existing != null)
            {
                _context.CommentLikes.Remove(existing);
                await _context.SaveChangesAsync();
                return Ok(new { liked = false });
            }

            var like = new CommentLike
            {
                ReviewId = dto.ReviewId,
                PlayerId = dto.PlayerId,
                CreatedAt = DateTime.UtcNow
            };

            _context.CommentLikes.Add(like);
            await _context.SaveChangesAsync();
            return Ok(new { liked = true });
        }

        [HttpGet("has-liked/{reviewId}/{playerId}")]
        public async Task<ActionResult<bool>> HasLiked(int reviewId, int playerId)
        {
            var hasLiked = await _context.CommentLikes
                .AnyAsync(cl => cl.ReviewId == reviewId && cl.PlayerId == playerId);

            return Ok(hasLiked);
        }

        [HttpGet("count/{reviewId}")]
        public async Task<ActionResult<int>> GetLikeCount(int reviewId)
        {
            var count = await _context.CommentLikes
                .CountAsync(cl => cl.ReviewId == reviewId);

            return Ok(count);
        }
    }
}
