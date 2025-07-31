using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Rastro.Application.Abstractions;
using Rastro.Domain;

namespace RastroApi.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;

        public AuthController(IAuthService auth) => _auth = auth;

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var response = await _auth.LoginAsync(new User { Email = request.Email, PasswordHash = request.Password });
            return Ok(response);
        }
    }
}
