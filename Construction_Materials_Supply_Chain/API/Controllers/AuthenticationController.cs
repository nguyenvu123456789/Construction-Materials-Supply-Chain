using API.DTOs;
using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly ScmVlxdContext _context;

        public AuthenticationController(ScmVlxdContext context)
        {
            _context = context;
        }

        // POST: api/authentication/register
        [HttpPost("register")]
        public IActionResult Register(RegisterRequestDto request)
        {
            if (_context.Users.Any(u => u.UserName == request.UserName))
            {
                return BadRequest(new { Message = "Username already exists" });
            }

            string hashedPassword;
            using (var sha = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(request.Password);
                var hash = sha.ComputeHash(bytes);
                hashedPassword = Convert.ToBase64String(hash);
            }

            var user = new User
            {
                UserName = request.UserName,
                PasswordHash = hashedPassword,
                Email = request.Email,
                CreatedAt = DateTime.Now
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return Ok(new { Message = "User registered successfully" });
        }

        // POST: api/authentication/login
        [HttpPost("login")]
        public IActionResult Login(LoginRequestDto request)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserName == request.UserName);
            if (user == null)
                return Unauthorized(new { Message = "Invalid username or password" });

            string hashOfInput;
            using (var sha = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(request.Password);
                var hash = sha.ComputeHash(bytes);
                hashOfInput = Convert.ToBase64String(hash);
            }

            if (user.PasswordHash != hashOfInput)
            {
                return Unauthorized(new { Message = "Invalid username or password" });
            }

            return Ok(new { Message = "Login successful" });
        }
    }
}
