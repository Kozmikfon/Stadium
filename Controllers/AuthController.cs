using Microsoft.AspNetCore.Mvc;
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
            var token = GenerateJwtToken(user);
            return Ok(new { Token = token });
        }
        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub,user.Email),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                new Claim("userId",user.Id.ToString()),
                new Claim("role","User")
            };

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
