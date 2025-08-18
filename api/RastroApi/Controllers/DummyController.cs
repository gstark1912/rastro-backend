using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RastroApi.Controllers
{
    [ApiController]
    [Route("dummy")]
    public class DummyController : ControllerBase
    {
        [HttpGet("hello")]
        [Authorize] // <- requiere token válido
        public IActionResult Hello()
        {
            // Podés acceder a claims del usuario:
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var all = User.Claims.Select(c => new { c.Type, c.Value });

            return Ok(new { Message = $"Hola {email}, tu token funciona 🚀" });            
        }
    }
}
