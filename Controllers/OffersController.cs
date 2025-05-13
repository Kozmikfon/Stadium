using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stadyum.API.Data;
using Stadyum.API.Models;
using Stadyum.API.Models.DTOs;

namespace Stadyum.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OffersController : ControllerBase
    {
        private readonly StadyumDbContext _context;

        public OffersController(StadyumDbContext context)
        {
            _context = context;
        }

        // GET: api/Offers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OfferDTO>>> GetOffers()
        {
            var offers = await _context.Offers.ToListAsync();

            var offerDTOs = offers.Select(o => new OfferDTO
            {
                Id = o.Id,
                SenderId = o.SenderId,
                ReceiverId = o.ReceiverId,
                MatchId = o.MatchId,
                Status = o.Status
            }).ToList();

            return Ok(offerDTOs);
        }
        // ekleme
        // GET: api/Offers/byPlayer/{playerId}
        [HttpGet("byPlayer/{playerId}")]
        public async Task<ActionResult<IEnumerable<OfferDTO>>> GetOffersByPlayer(int playerId)
        {
            var offers = await _context.Offers
                .Where(o => o.ReceiverId == playerId)
                .ToListAsync();

            var offerDTOs = offers.Select(o => new OfferDTO
            {
                Id = o.Id,
                SenderId = o.SenderId,
                ReceiverId = o.ReceiverId,
                MatchId = o.MatchId,
                Status = o.Status
            }).ToList();

            return Ok(offerDTOs);
        }

        // GET: api/Offers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OfferDTO>> GetOffer(int id)
        {
            var offer = await _context.Offers.FindAsync(id);

            if (offer == null)
                return NotFound();

            var offerDTO = new OfferDTO
            {
                Id = offer.Id,
                SenderId = offer.SenderId,
                ReceiverId = offer.ReceiverId,
                MatchId = offer.MatchId,
                Status = offer.Status
            };

            return Ok(offerDTO);
        }

        // POST: api/Offers
        [HttpPost]
        public async Task<ActionResult<OfferDTO>> CreateOffer(OfferCreateDTO dto)
        {
            var offer = new Offer
            {
                SenderId = dto.SenderId,
                ReceiverId = dto.ReceiverId,
                MatchId = dto.MatchId,
                Status = dto.Status
            };


            _context.Offers.Add(offer);
            await _context.SaveChangesAsync();

            var result = new OfferDTO
            {
                Id = offer.Id,
                SenderId = offer.SenderId,
                ReceiverId = offer.ReceiverId,
                MatchId = offer.MatchId,
                Status = offer.Status
            };

            return CreatedAtAction(nameof(GetOffer), new { id = offer.Id }, result);
        }

        // PUT: api/Offers/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOffer(int id, OfferCreateDTO dto)
        {
            var offer = await _context.Offers.FindAsync(id);

            if (offer == null)
                return NotFound();

            offer.SenderId = dto.SenderId;
            offer.ReceiverId = dto.ReceiverId;
            offer.MatchId = dto.MatchId;
            offer.Status = dto.Status;

            await _context.SaveChangesAsync();

            return NoContent();
        }
        //ekleme update
        [HttpPut("update-status/{id}")]
        public async Task<IActionResult> UpdateOfferStatus(int id, [FromBody] OfferStatusUpdateDTO dto)
        {
            try
            {
                Console.WriteLine($"🟢 Status güncelleme isteği: ID={id}, YeniStatus={dto.Status}");

                var validStatuses = new[] { "Pending", "Accepted", "Rejected" };
                if (!validStatuses.Contains(dto.Status))
                    return BadRequest("Invalid status.");

                var offer = await _context.Offers.FindAsync(id);
                if (offer == null)
                    return NotFound();

                offer.Status = dto.Status;
                _context.Entry(offer).Property(o => o.Status).IsModified = true;

                Console.WriteLine($"⏳ SaveChanges'e geçiliyor...");

                var result = await _context.SaveChangesAsync();
                Console.WriteLine($"✅ Kaydedildi. Etkilenen satır sayısı: {result}");

                return Ok(new OfferDTO
                {
                    Id = offer.Id,
                    SenderId = offer.SenderId,
                    ReceiverId = offer.ReceiverId,
                    MatchId = offer.MatchId,
                    Status = offer.Status
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ SaveChanges hatası: {ex.Message}");
                return StatusCode(500, "Bir hata oluştu: " + ex.Message);
            }
        }

        //matchıd
        [HttpGet("accepted-by-match/{matchId}")]
        public async Task<ActionResult<IEnumerable<OfferDTO>>> GetAcceptedOffersByMatch(int matchId)
        {
            var offers = await _context.Offers
                .Where(o => o.MatchId == matchId && o.Status == "Accepted")
                .ToListAsync();

            var offerDTOs = offers.Select(o => new OfferDTO
            {
                Id = o.Id,
                SenderId = o.SenderId,
                ReceiverId = o.ReceiverId,
                MatchId = o.MatchId,
                Status = o.Status,
                ReceiverName = _context.Players
                    .Where(p => p.Id == o.ReceiverId)
                    .Select(p => p.FirstName + " " + p.LastName)
                    .FirstOrDefault()
            }).ToList();

            return Ok(offerDTOs);
        }






        // DELETE: api/Offers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOffer(int id)
        {
            var offer = await _context.Offers.FindAsync(id);

            if (offer == null)
                return NotFound();

            _context.Offers.Remove(offer);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
