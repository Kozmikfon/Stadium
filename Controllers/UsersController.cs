using Microsoft.AspNetCore.Mvc;
using Stadyum.API.Models;
using Stadyum.API.Data;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;


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
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetUsers()
        {
            var users = _context.Users.ToList();
            return Ok(users);
        }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult AddUser(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
            return Ok(user);
        }
        [HttpGet("secure")]
        [Authorize]
        public IActionResult GetSecureData() { 
            var userId = User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
            var email = User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
            return Ok(new
            { 
                Message= "Bu endpoint başarılı!",
                UserId = userId,
                Email = email
            });
        }
    }
}
