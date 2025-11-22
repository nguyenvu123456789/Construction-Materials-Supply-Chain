using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;

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

            var results = new List<AuthResponseDto>();

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                using var package = new ExcelPackage(stream);
                var ws = package.Workbook.Worksheets[0];

                var row = 2;
                while (true)
                {
                    var email = ws.Cells[row, 1].Text?.Trim();
                    if (string.IsNullOrWhiteSpace(email))
                        break;

                    var fullName = ws.Cells[row, 2].Text?.Trim();
                    var phone = ws.Cells[row, 3].Text?.Trim();
                    var status = ws.Cells[row, 4].Text?.Trim();
                    var partnerText = ws.Cells[row, 5].Text?.Trim();

                    int? partnerId = null;
                    if (int.TryParse(partnerText, out var p))
                        partnerId = p;

                    var dto = new AdminCreateUserRequestDto
                    {
                        Email = email,
                        FullName = fullName,
                        Phone = phone,
                        Status = status,
                        PartnerId = partnerId
                    };

                    try
                    {
                        var created = _auth.AdminCreateUser(dto);
                        results.Add(created);
                    }
                    catch
                    {
                    }

                    row++;
                }
            }

            return Ok(results);
        }

        [HttpPost("otp/request")]
        public async Task<IActionResult> RequestOtp([FromBody] OtpRequestDto request)
        {
            await _auth.RequestOtpAsync(request);
            return Ok();
        }

        [HttpPost("otp/verify")]
        public async Task<ActionResult<bool>> VerifyOtp([FromBody] OtpVerifyDto request)
        {
            var result = await _auth.VerifyOtpAsync(request);
            return Ok(result);
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto request)
        {
            await _auth.ForgotPasswordRequestAsync(request);
            return Ok();
        }

        [HttpPost("forgot-password/reset")]
        public async Task<IActionResult> ResetPasswordWithOtp([FromBody] ResetPasswordWithOtpDto request)
        {
            await _auth.ResetPasswordWithOtpAsync(request);
            return Ok();
        }
    }
}
