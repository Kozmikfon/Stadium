using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Stadyum.API.Config;
using Microsoft.AspNetCore.Http.HttpResults;



namespace Stadyum.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController:ControllerBase
    {
        private readonly JwtSettings _jwtSettings;
        public AuthController(IConfiguration configuration)
        {
            _jwtSettings = configuration.GetSection("Jwt").Get<JwtSettings>();
        }
        [HttpPost("login")]
        public IActionResult Login(string email,string password)
        {
            // basit doğrulama veritabanı üzerinden kontrol yapacağız
            if(email=="demo@gmail.com" && password == "12345")
            {
                var token = GenerateJwtToken(email);
                return Ok(new {Token=token });
            }
            return Unauthorized("Kullanıcı adı veya şifre yanlış");
        }
        private string GenerateJwtToken(string email) 
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub,email),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.key));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken
                (
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(_jwtSettings.ExpirationInMinutes),
                signingCredentials: credentials 
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
