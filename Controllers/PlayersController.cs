using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stadyum.API.Data;
using Stadyum.API.Models;
using Stadyum.API.Models.DTOs;

namespace Stadyum.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayersController : ControllerBase
    {
        private readonly StadyumDbContext _context;

        public PlayersController(StadyumDbContext context)
        {
            _context = context;
        }



        // 🔹 Tüm oyuncuları DTO ile getir
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlayerDTO>>> GetPlayers()
        {
            var players = await _context.Players
                .Include(p => p.Team)
                .Select(p => new PlayerDTO
                {
                    Id = p.Id,
                    FirstName = p.FirstName,
                    LastName = p.LastName,
                    Email = p.Email,
                    Position = p.Position,
                    SkillLevel = p.SkillLevel,
                    Rating = p.Rating,
                    CreateAd = p.CreateAd,
                    TeamId = p.TeamId,
                    TeamName = p.Team != null ? p.Team.Name : null
                })
                .ToListAsync();

            return Ok(players);
        }


        // 🔹 Belirli oyuncuyu DTO ile getir
        [HttpGet("{id}")]
        public async Task<ActionResult<PlayerDTO>> GetPlayer(int id)
        {
            var player = await _context.Players
                .Include(p => p.Team)
                .Where(p => p.Id == id)
                .Select(p => new PlayerDTO
                {
                    Id = p.Id,
                    FirstName = p.FirstName,
                    LastName = p.LastName,
                    Email = p.Email,
                    Position = p.Position,
                    SkillLevel = p.SkillLevel,
                    Rating = p.Rating,
                    CreateAd = p.CreateAd,
                    TeamId = p.TeamId,
                    TeamName = p.Team != null ? p.Team.Name : null
                })
                .FirstOrDefaultAsync();

            if (player == null)
                return NotFound();

            return Ok(player);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> PutPlayer(int id, PlayerUpdateDTO dto)
        {
            var player = await _context.Players.FindAsync(id);
            if (player == null)
                return NotFound();

            // Sadece gerekli alanlar güncelleniyor
            player.Email = dto.Email;
            player.Position = dto.Position;

            _context.Entry(player).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlayerExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // istatistik
        [HttpGet("stats/{playerId}")]
        public async Task<IActionResult> GetPlayerStats(int playerId)
        {
            var player = await _context.Players.FindAsync(playerId);
            if (player == null)
                return NotFound();

            // Oyuncunun takımına ait maç sayısı
            int totalMatches = await _context.Matches
                .CountAsync(m => m.Team1Id == player.TeamId || m.Team2Id == player.TeamId);

            // Bu oyuncuya gönderilen toplam teklif sayısı
            int totalOffers = await _context.Offers
                .CountAsync(o => o.ReceiverId == player.Id);

            // Takıma katılalı kaç gün olmuş
            int membershipDays = (int)(DateTime.Now - player.CreateAd).TotalDays;

            // Bu oyuncuya yapılan değerlendirmelerin ortalama puanı
            double avgRating = await _context.Reviews
                .Where(r => r.ReviewedUserId == player.Id && r.Rating > 0)
                .AverageAsync(r => (double?)r.Rating) ?? 0;

            // JSON olarak döndür
            return Ok(new
            {
                totalMatches,
                totalOffers,
                averageRating = Math.Round(avgRating, 1),
                membershipDays
            });
        }

       





        // 🔹 Yeni oyuncu oluştur
        [HttpPost]
        public async Task<ActionResult<Player>> PostPlayer(PlayerCreateDTO dto)
        {
            var player = new Player
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Position = dto.Position,
                SkillLevel = dto.SkillLevel,
                Rating = dto.Rating,
                CreateAd = dto.CreateAd,
                TeamId = dto.TeamId,
                UserId = dto.UserId
            };

            _context.Players.Add(player);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPlayer), new { id = player.Id }, player);
        }

        // GET: api/Players/byUser/{userId}
        [HttpGet("byUser/{userId}")]
        public async Task<ActionResult<PlayerDTO>> GetPlayerByUserId(int userId)
        {
            var player = await _context.Players
                            .Include(p => p.Team)
                            .FirstOrDefaultAsync(p => p.UserId == userId);

            if (player == null)
                return NotFound("Bu kullanıcıya ait oyuncu bulunamadı.");

            // PlayerDTO dönüşümü burada yapılıyor olmalı
            var playerDTO = new PlayerDTO
            {
                Id = player.Id,
                FirstName = player.FirstName,
                LastName = player.LastName,
                Email = player.Email,
                Position = player.Position,
                SkillLevel = player.SkillLevel,
                Rating = player.Rating,
                CreateAd = player.CreateAd,
                TeamId = player.TeamId,
                TeamName = player.Team != null ? player.Team.Name : null
            };

            return Ok(playerDTO);
        }


        // 🔹 Oyuncu sil
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlayer(int id)
        {
            var player = await _context.Players
                .Include(p => p.Team) // İlişkili takımı da getir
                .FirstOrDefaultAsync(p => p.Id == id);

            if (player == null)
                return NotFound();

            // Bu oyuncu bir takımın kaptanıysa, kaptanlığı kaldır
            var teamWhereCaptain = await _context.Teams.FirstOrDefaultAsync(t => t.CaptainId == id);
            if (teamWhereCaptain != null)
            {
                teamWhereCaptain.CaptainId = null;
            }

            _context.Players.Remove(player);
            await _context.SaveChangesAsync();

            return NoContent();
        }



        private bool PlayerExists(int id)
        {
            return _context.Players.Any(e => e.Id == id);
        }
    }
}
