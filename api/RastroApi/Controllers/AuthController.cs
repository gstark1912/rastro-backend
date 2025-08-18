using Microsoft.AspNetCore.Mvc;
using Rastro.Application.Abstractions;
using Rastro.Application.Contracts.Auth;

namespace RastroApi.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;

        public AuthController(IAuthService auth) => _auth = auth;

        /// <summary>
        /// Login with email & password (plaintext over TLS).
        /// Returns JWT (AuthResponse).
        /// </summary>
        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
        {
            // ModelState validation comes from [ApiController]
            var response = await _auth.LoginAsync(request);
            return Ok(response);
        }

        /// <summary>
        /// Register a new user and returns JWT (AuthResponse).
        /// </summary>
        [HttpPost("register")]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
        {
            var response = await _auth.RegisterAsync(request);
            return Ok(response);
        }
    }
}
