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

        [HttpPost("admin/create-user")]
        public ActionResult<AuthResponseDto> AdminCreateUser([FromBody] AdminCreateUserRequestDto request)
        {
            var result = _auth.AdminCreateUser(request);
            return Ok(result);
        }

        [HttpPost("change-password")]
        public IActionResult ChangePassword([FromBody] ChangePasswordRequestDto request)
        {
            _auth.ChangePassword(request);
            return Ok(new { Message = "Password changed successfully" });
        }

        [HttpPost("admin/bulk-create")]
        public async Task<ActionResult<List<AuthResponseDto>>> BulkCreateUsers([FromBody] BulkCreateUsersByEmailRequestDto request)
        {
            var result = await _auth.BulkCreateUsersByEmailAsync(request);
            return Ok(result);
        }

        [HttpPost("admin/bulk-create-excel")]
        public async Task<ActionResult<List<AuthResponseDto>>> BulkCreateUsersFromExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File is required");

            var emails = new List<string>();

            using (var stream = file.OpenReadStream())
            using (var package = new OfficeOpenXml.ExcelPackage(stream))
            {
                var sheet = package.Workbook.Worksheets[0];
                var row = 2;
                while (true)
                {
                    var value = sheet.Cells[row, 1].Text;
                    if (string.IsNullOrWhiteSpace(value)) break;
                    emails.Add(value);
                    row++;
                }
            }

            var dto = new BulkCreateUsersByEmailRequestDto { Emails = emails };
            var result = await _auth.BulkCreateUsersByEmailAsync(dto);
            return Ok(result);
        }
    }
}
