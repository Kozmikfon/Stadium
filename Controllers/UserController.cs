using Microsoft.AspNetCore.Mvc;
using Stadyum.API.Models;
using Stadyum.API.Data;


namespace Stadyum.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")] 

    public class UsersContollers:ControllerBase
    {
        private readonly StadyumDbContext _context;
        public UsersContollers(StadyumDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult GetUsers()
        {
            var users = _context.Users.ToList();
            return Ok(users);
        }
        [HttpPost]
        public IActionResult AddUser(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
            return Ok(user);
        }
    }
}
