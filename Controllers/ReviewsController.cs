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
                CreateDate = r.CreateDate
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
                CreateDate = review.CreateDate
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
                CreateDate = DateTime.UtcNow
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
                CreateDate = review.CreateDate
            };

            return CreatedAtAction(nameof(GetReview), new { id = review.Id }, reviewDTO);
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
