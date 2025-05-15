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
        public async Task<IActionResult> GetOffersByPlayer(int playerId)
        {
            var offers = await _context.Offers
                .Include(o => o.Match)
                    .ThenInclude(m => m.Team1) // kaptan Team1'de olabilir
                .Include(o => o.Match.Team1.Captain) // kaptan bilgisi için
                .Where(o => o.ReceiverId == playerId)
                .Select(o => new OfferDTO
                {
                    Id = o.Id,
                    SenderId = o.SenderId,
                    ReceiverId = o.ReceiverId,
                    MatchId = o.MatchId,
                    Status = o.Status,

                    // 👇 Eklenen alanlar
                    FieldName = o.Match.FieldName,
                    MatchDate = o.Match.MatchDate,
                    CaptainName = o.Match.Team1.Captain.FirstName + " " + o.Match.Team1.Captain.LastName
                })
                .ToListAsync();

            return Ok(offers);
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



        // maça teklif
        [HttpGet("count-accepted/{matchId}")]
        public async Task<IActionResult> GetAcceptedCount(int matchId)
        {
            var count = await _context.Offers
                .Where(o => o.MatchId == matchId && o.Status == "Accepted")
                .CountAsync();

            return Ok(count);
        }


        [HttpDelete("{offerId}")]
        public async Task<IActionResult> RemoveOffer(int offerId)
        {
            var offer = await _context.Offers.FindAsync(offerId);
            if (offer == null)
                return NotFound();

            var match = await _context.Matches.FindAsync(offer.MatchId);
            if (match == null)
                return NotFound();

            var team = await _context.Teams.FindAsync(match.Team1Id); // 👈 sadece Team1 kaptanı yetkili

            if (team == null)
                return NotFound();

            // 🔐 JWT'den giriş yapan kişi kim?
            var playerIdClaim = User.Claims.FirstOrDefault(c => c.Type == "playerId")?.Value;
            if (playerIdClaim == null)
                return Unauthorized();

            var currentPlayerId = int.Parse(playerIdClaim);

            // ❌ Eğer kaptan değilse → yetki yok
            if (currentPlayerId != team.CaptainId)
                return Forbid("Bu işlemi sadece kaptan yapabilir.");

            _context.Offers.Remove(offer);
            await _context.SaveChangesAsync();

            return Ok("Oyuncu başarıyla maçtan çıkarıldı.");
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

        [HttpGet("accepted/{playerId}")]
        public async Task<IActionResult> GetAcceptedOffers(int playerId)
        {
            var offers = await _context.Offers
                .Where(o => o.ReceiverId == playerId && o.Status == "Kabul Edildi")
                .Include(o => o.Match)
                .ToListAsync();

            return Ok(offers.Select(o => new {
                o.Id,
                o.MatchId,
                FieldName = o.Match.FieldName,
                MatchDate = o.Match.MatchDate
            }));
        }

        [HttpGet("rejected/{playerId}")]
        public async Task<IActionResult> GetRejectedOffers(int playerId)
        {
            var offers = await _context.Offers
                .Where(o => o.ReceiverId == playerId && o.Status == "Reddedildi")
                .Include(o => o.Match)
                .ToListAsync();

            return Ok(offers.Select(o => new {
                o.Id,
                o.MatchId,
                FieldName = o.Match.FieldName,
                MatchDate = o.Match.MatchDate
            }));
        }

    }
}
