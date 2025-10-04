using API.DTOs;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authService;

        public AuthenticationController(IAuthenticationService authService)
        {
            _authService = authService;
        }

        // POST: api/authentication/register
        [HttpPost("register")]
        public IActionResult Register(RegisterRequestDto request)
        {
            try
            {
                var user = _authService.Register(request.UserName, request.Password, request.Email);
                return Ok(new { Message = "User registered successfully", UserId = user.UserId });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        // POST: api/authentication/login
        [HttpPost("login")]
        public IActionResult Login(LoginRequestDto request)
        {
            var user = _authService.Login(request.UserName, request.Password);
            if (user == null)
                return Unauthorized(new { Message = "Invalid username or password" });

            return Ok(new { Message = "Login successful", UserId = user.UserId, UserName = user.UserName });
        }

        // POST: api/authentication/logout
        [HttpPost("logout")]
        public IActionResult Logout([FromBody] int userId)
        {
            _authService.Logout(userId);
            return Ok(new { Message = "Logout successful" });
        }
    }
}
