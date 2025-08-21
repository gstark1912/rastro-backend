using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RastroApi.Controllers
{
    [ApiController]
    [Route("dummy")]
    public class DummyController : RastroControllerBase
    {
        [HttpGet("hello")]
        [Authorize] // <- requiere token válido
        public IActionResult Hello()
        {
            var email = this.UserEmail;
            return Ok(new { Message = $"Hola {email}, tu token funciona 🚀" });            
        }
    }
}
