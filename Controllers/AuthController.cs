﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Stadyum.API.Config;
using Stadyum.API.Models;
using Stadyum.API.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using LoginRequest = Stadyum.API.Models.LoginRequest;
using Microsoft.EntityFrameworkCore;



namespace Stadyum.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly JwtSettings _jwtSettings;
        private readonly StadyumDbContext _context;
        public AuthController(IConfiguration configuration, StadyumDbContext context)
        {
            _jwtSettings = configuration.GetSection("Jwt").Get<JwtSettings>();
            _context = context;
        }


        //register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            if (string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.PasswordHash))
            {
                return BadRequest("Email ve şifre zorunludur.");
            }

            // Email benzersiz mi kontrol et
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
            if (existingUser != null)
            {
                return BadRequest("Bu email zaten kullanılıyor.");
            }

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Kullanıcı başarıyla kaydedildi." });
        }

        //sifre degistir
        // AuthController.cs içerisine ekle
        [HttpPut("change-password/{userId}")]
        public async Task<IActionResult> ChangePassword(int userId, [FromBody] string newPassword)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return NotFound();

            user.PasswordHash = newPassword;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return Ok("Şifre güncellendi.");
        }



        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            Console.WriteLine($"Gelen Email: {request.Email}");
            Console.WriteLine($"Gelen Password: {request.Password}");

            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest("Email veya şifre boş olamaz!");
            }

            var user = _context.Users.FirstOrDefault(u => u.Email == request.Email && u.PasswordHash == request.Password);
            if (user == null)
            {
                return Unauthorized("Kullanıcı adı veya şifre hatalı");
            }
            // Kullanıcının Player kaydı var mı kontrol et
            var player = _context.Players.FirstOrDefault(p => p.UserId == user.Id);
            if (player == null)
            {
                var newPlayer = new Player
                {
                    UserId = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Position = user.Position,
                    SkillLevel = user.SkillLevel,
                    Rating = user.Rating,
                    CreateAd = DateTime.UtcNow
                };

                _context.Players.Add(newPlayer);
                _context.SaveChanges();
                player = newPlayer; // ✅ En önemli satır!
            }



            string role = player?.IsAdmin == true ? "Admin" : "Player";

            var token = GenerateJwtToken(user, player?.Id, role);

            return Ok(new { Token = token, Role = role }); // Küçük harf dikkat!
        }



        private string GenerateJwtToken(User user, int? playerId,string role)
        {
            var claims = new List<Claim>
{
             new Claim(JwtRegisteredClaimNames.Sub, user.Email),
             new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
             new Claim("userId", user.Id.ToString()),
             new Claim("playerId", playerId?.ToString() ?? ""),
             new Claim("role", role), // 🔥 bunu eklersen token içinden rahat çekersin
             new Claim(ClaimTypes.Role, role) // eski hali
};  



            if (playerId.HasValue)
            {
                claims.Add(new Claim("playerId", playerId.Value.ToString()));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken
                (
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(_jwtSettings.ExpirationInMinutes),
                signingCredentials: credentials
                );
            Console.WriteLine("Oluşturulan Token: " + new JwtSecurityTokenHandler().WriteToken(token));

            return new JwtSecurityTokenHandler().WriteToken(token);
        }       
    }
}
