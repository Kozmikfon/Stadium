using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stadyum.API.Data;
using Stadyum.API.Models;
using Stadyum.API.Models.DTOs;

namespace Stadyum.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly StadyumDbContext _context;

        public ReviewsController(StadyumDbContext context)
        {
            _context = context;
        }

        // GET: api/Reviews
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReviewDTO>>> GetReviews()
        {
            var reviews = await _context.Reviews.ToListAsync();

            var reviewDTOs = reviews.Select(r => new ReviewDTO
            {
                Id = r.Id,
                MatchId = r.MatchId,
                ReviewerId = r.ReviewerId,
                ReviewedUserId = r.ReviewedUserId,
                ReviewedTeamId = r.ReviewedTeamId,
                Comment = r.Comment,
                Rating = r.Rating,
                
            }).ToList();

            return Ok(reviewDTOs);
        }

        // GET: api/Reviews/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ReviewDTO>> GetReview(int id)
        {
            var review = await _context.Reviews.FindAsync(id);

            if (review == null)
                return NotFound();

            var reviewDTO = new ReviewDTO
            {
                Id = review.Id,
                MatchId = review.MatchId,
                ReviewerId = review.ReviewerId,
                ReviewedUserId = review.ReviewedUserId,
                ReviewedTeamId = review.ReviewedTeamId,
                Comment = review.Comment,
                Rating = review.Rating,
                
            };

            return Ok(reviewDTO);
        }

        // POST: api/Reviews
        [HttpPost]
        public async Task<ActionResult<ReviewDTO>> CreateReview(ReviewCreateDTO dto)
        {
            var review = new Review
            {
                MatchId = dto.MatchId,
                ReviewerId = dto.ReviewerId,
                ReviewedUserId = dto.ReviewedUserId,
                ReviewedTeamId = dto.ReviewedTeamId,
                Comment = dto.Comment,
                Rating = dto.Rating,
               
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            var reviewDTO = new ReviewDTO
            {
                Id = review.Id,
                MatchId = review.MatchId,
                ReviewerId = review.ReviewerId,
                ReviewedUserId = review.ReviewedUserId,
                ReviewedTeamId = review.ReviewedTeamId,
                Comment = review.Comment,
                Rating = review.Rating,
                
            };

            return CreatedAtAction(nameof(GetReview), new { id = review.Id }, reviewDTO);
        }

        // tüm yorumları getir
        [HttpGet("byUser/{playerId}")]
        public async Task<IActionResult> GetReviewsByPlayer(int playerId)
        {
            var comments = await _context.Reviews
                .Where(r => r.ReviewedUserId == playerId)
                .Include(r => r.Match)
                .ToListAsync();

            return Ok(comments.Select(r => new {
                r.Id,
                r.MatchId,
                r.Comment,
                r.Rating,
                MatchDate = r.Match.MatchDate,
                MatchField = r.Match.FieldName
            }));
        }

        //oyuncuya özel yorum
        [HttpGet("mentions/{playerName}")]
        public async Task<IActionResult> GetMentions(string playerName)
        {
            if (string.IsNullOrWhiteSpace(playerName))
                return BadRequest("Geçerli bir oyuncu adı girilmelidir.");

            string mentionTag = "@" + playerName.ToLower();

            var mentions = await _context.Reviews
                .Where(r => r.Comment.ToLower().Contains(mentionTag))
                .Include(r => r.Match)
                .ToListAsync();

            var results = mentions.Select(r => new
            {
                r.MatchId,
                r.Comment,
                r.Rating,
                MatchDate = r.Match.MatchDate,
                FieldName = r.Match.FieldName
            });

            return Ok(results);
        }



        // DELETE: api/Reviews/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReview(int id)
        {
            var review = await _context.Reviews.FindAsync(id);

            if (review == null)
                return NotFound();

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
