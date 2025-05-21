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
            var offers = await _context.Offers
                .Include(o => o.Sender)
                .Include(o => o.Receiver)
                .Include(o => o.Match)
                .ToListAsync();

            var offerDTOs = offers.Select(o => new OfferDTO
            {
                Id = o.Id,
                SenderId = o.SenderId,
                ReceiverId = o.ReceiverId,
                MatchId = o.MatchId,
                Status = o.Status,

                // Ekstra veri:
                ReceiverName = o.Receiver != null ? $"{o.Receiver.FirstName} {o.Receiver.LastName}" : null,
                //SenderName = o.Sender != null ? $"{o.Sender.FirstName} {o.Sender.LastName}" : null,
                MatchDate = o.Match?.MatchDate ?? DateTime.MinValue,
                FieldName = o.Match?.FieldName,
                CaptainName = o.Match?.Team1?.Captain?.FirstName // varsa böyle detay istenebilir
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
                    .ThenInclude(m => m.Team1)
                .Include(o => o.Match.Team1.Captain)
                .Include(o => o.Sender) // ✅ sender bilgisi için eklendi
                .Where(o => o.ReceiverId == playerId)
                .Select(o => new OfferDTO
                {
                    Id = o.Id,
                    SenderId = o.SenderId,
                    ReceiverId = o.ReceiverId,
                    MatchId = o.MatchId,
                    Status = o.Status,

                    FieldName = o.Match.FieldName,
                    MatchDate = o.Match.MatchDate,
                    CaptainName = o.Match.Team1.Captain.FirstName + " " + o.Match.Team1.Captain.LastName,

                    MatchTeamName = o.Match.Team1.Name,
                    MatchFieldName = o.Match.FieldName,
                    MatchCaptainName = o.Match.Team1.Captain.FirstName + " " + o.Match.Team1.Captain.LastName,

                    SenderName = o.Sender.FirstName + " " + o.Sender.LastName // ✅ yeni eklenen alan
                })
                .ToListAsync();

            return Ok(offers);
        }



        [HttpGet("byCaptain/{playerId}")]
        public async Task<IActionResult> GetOffersByCaptain(int playerId)
        {
            var team = await _context.Teams
                .FirstOrDefaultAsync(t => t.CaptainId == playerId);

            if (team == null)
                return NotFound("Kaptan takımı bulunamadı.");

            // O takımın oynayacağı tüm maçlar
            var matchIds = await _context.Matches
                .Where(m => m.Team1Id == team.Id || m.Team2Id == team.Id)
                .Select(m => m.Id)
                .ToListAsync();

            var offers = await _context.Offers
                .Include(o => o.Sender)
                .Include(o => o.Match)
                .Where(o => matchIds.Contains(o.MatchId))
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
            if (dto.SenderId <= 0 || dto.MatchId <= 0)
                return BadRequest("SenderId ve MatchId zorunludur.");

            // ✅ Eğer receiverId gerekli değilse bu kontrolü kaldır
            // Not: eğer sadece belirli durumlarda gerekliyse, özel kontrol yap
            // Örneğin: if (dto.Type == "Direct") { receiverId zorunlu }
            // ✅ Mevcut aynı teklif kontrolü:


            bool alreadyExists = await _context.Offers.AnyAsync(o =>
                o.SenderId == dto.SenderId &&
                o.MatchId == dto.MatchId &&
                o.ReceiverId == dto.ReceiverId // hem oyuncuya hem maça özel kontrolü kapsar
            );

            if (alreadyExists)
                return BadRequest("Bu teklifi zaten gönderdiniz.");
            var offer = new Offer
            {
                SenderId = dto.SenderId,
                ReceiverId = dto.ReceiverId, // null olabilir
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

            if (dto.SenderId <= 0 || dto.MatchId <= 0 || dto.ReceiverId == null)
                return BadRequest("Geçersiz veri: Tüm alanlar doldurulmalıdır.");

            offer.SenderId = dto.SenderId;
            offer.ReceiverId = dto.ReceiverId.Value; // null değilse güvenli
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
            var groupedOffers = await _context.Offers
                .Where(o => o.MatchId == matchId && o.Status == "Accepted" && o.ReceiverId != null)
                .GroupBy(o => o.ReceiverId)
                .ToListAsync();


            var latestOffers = groupedOffers
                .Select(g => g.OrderByDescending(o => o.Id).FirstOrDefault())
                .Where(o => o != null) // 💥 null gelenleri filtrele
                .ToList();

            var offerDTOs = latestOffers.Select(o =>
            {
                string? receiverName = null;

                if (o!.ReceiverId.HasValue) // o artık null değil ama yine ! kullan
                {
                    var receiver = _context.Players
                        .Where(p => p.Id == o.ReceiverId.Value)
                        .Select(p => new { p.FirstName, p.LastName })
                        .FirstOrDefault();

                    if (receiver != null)
                        receiverName = $"{receiver.FirstName} {receiver.LastName}";
                }

                return new OfferDTO
                {
                    Id = o.Id,
                    SenderId = o.SenderId,
                    ReceiverId = o.ReceiverId,
                    MatchId = o.MatchId,
                    Status = o.Status,
                    ReceiverName = receiverName
                };
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
