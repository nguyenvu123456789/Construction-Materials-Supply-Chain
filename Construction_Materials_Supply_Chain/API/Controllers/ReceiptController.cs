using Application.DTOs;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReceiptController : ControllerBase
    {
        private readonly IReceiptService _receiptService;

        public ReceiptController(IReceiptService receiptService)
        {
            _receiptService = receiptService;
        }

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> CreateReceiptAsync([FromBody] ReceiptCreateDto receiptCreateDto, [FromHeader] string createdBy, [FromRoute] int partnerId)
        {
            try
            {
                var receiptDto = await _receiptService.CreateReceiptAsync(receiptCreateDto, partnerId, createdBy);
                return Ok(receiptDto);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
