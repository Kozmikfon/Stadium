using Microsoft.AspNetCore.Mvc;
using Stadyum.API.Models;
using Stadyum.API.Data;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;



namespace Stadyum.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")] 

    public class UsersController : ControllerBase
    {
        private readonly StadyumDbContext _context;
        public UsersController(StadyumDbContext context)
        {
            _context = context;
        }

        [HttpGet]        
        public IActionResult GetUsers()
        {
            var users = _context.Users
                .AsNoTracking() // İzlemeyi kapatıyoruz
                .ToList();

            if (!users.Any())
            {
                return NotFound("Hiç kullanıcı bulunamadı!");
            }

            return Ok(users);
        }

        [HttpPost]
        
        public IActionResult AddUser(User user)
        {
            _context.Users.Add(user);
            var result = _context.SaveChanges();

            if (result > 0)
            {
                return Ok(user);
            }
            return BadRequest("Kullanıcı kaydedilemedi.");
        }
        [HttpGet("secure")]
        [Authorize]
        public IActionResult GetSecureData() {

            var token = Request.Headers["Authorization"];
            Console.WriteLine($"Gelen Token: {token}");

            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized("Token gelmedi veya eksik.");
            }
            var userId = User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
            var email = User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
            var role = User.Claims.FirstOrDefault(c => c.Type == "role")?.Value;
            
            
            //Console.WriteLine($"Token Doğrulandı: UserId = {userId}, Email = {email}, Role = {role}");
            if (userId == null || email == null)
            {
                return Unauthorized("Token geçersiz veya claim eksik!");
            }
            return Ok(new
            { 
                Message= "Bu endpoint başarılı!",
                UserId = userId,
                Email = email,
                Role = role
            });    

        }
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }


        [HttpGet("admin")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetAdminData()
        {
            return Ok("Bu endpoint'e sadece Admin erişebilir!");
        }

    }
}
