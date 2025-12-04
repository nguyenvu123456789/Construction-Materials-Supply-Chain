using Application.DTOs;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> CreatePaymentAsync([FromBody] PaymentCreateDto paymentCreateDto, [FromHeader] string createdBy, [FromRoute] int partnerId)
        {
            try
            {
                var paymentDto = await _paymentService.CreatePaymentAsync(paymentCreateDto, partnerId, createdBy);
                return Ok(paymentDto);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
