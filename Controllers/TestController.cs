using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Stadyum.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
 

        [HttpGet("public")]
        public IActionResult PublicEndpoint()
        {
            return Ok("🌍 Bu endpoint herkese açık.");
        }
        [Authorize]
        [HttpGet("test-token")]
        public IActionResult TestToken()
        {
            return Ok("✅ Token başarıyla doğrulandı.");
        }
        [Authorize]
        [HttpGet("secure")]
        public IActionResult SecureEndpoint()
        {
            return Ok("Token geçerli, admin giriş başarılı.");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin-only")]
        public IActionResult AdminOnlyEndpoint()
        {
            return Ok("Admin yetkisiyle erişildi.");
        }


    }
}
