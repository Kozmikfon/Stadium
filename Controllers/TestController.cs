using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;




namespace Stadyum.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController: ControllerBase
    {
        [HttpGet("secure")]
        [Authorize]
        public IActionResult GetSecureData()
        {
            return Ok("bu endpoint başarılı!");
        }
    }
}
