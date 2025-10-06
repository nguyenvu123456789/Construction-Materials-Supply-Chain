using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _auth;

        public AuthenticationController(IAuthenticationService auth)
        {
            _auth = auth;
        }

        [HttpPost("register")]
        public ActionResult<AuthResponseDto> Register([FromBody] RegisterRequestDto request)
            => Ok(_auth.Register(request));

        [HttpPost("login")]
        public ActionResult<AuthResponseDto> Login([FromBody] LoginRequestDto request)
        {
            var result = _auth.Login(request);
            return result is null ? Unauthorized(new { Message = "Invalid username or password" }) : Ok(result);
        }

        [HttpPost("logout")]
        public IActionResult Logout([FromBody] int userId)
        {
            _auth.Logout(userId);
            return Ok(new { Message = "Logout successful" });
        }
    }
}
