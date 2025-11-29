using Application.Common.Pagination;
using Application.DTOs.Application.DTOs;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/price-catalog")]
    public class PriceCatalogController : ControllerBase
    {
        private readonly IPriceMaterialPartnerService _service;
        public PriceCatalogController(IPriceMaterialPartnerService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResultDto<PriceMaterialPartnerDto>>> Get([FromQuery] PriceCatalogQueryDto q)
        {
            var res = await _service.GetAllAsync(q);
            return Ok(res);
        }

        [HttpGet("buyer/{buyerPartnerId}/seller/{sellerPartnerId}")]
        public async Task<IActionResult> GetPricesForPartner(int buyerPartnerId, int sellerPartnerId)
        {
            if (buyerPartnerId <= 0 || sellerPartnerId <= 0)
                return BadRequest("PartnerId không hợp lệ");

            var prices = await _service.GetPricesForPartnerAsync(buyerPartnerId, sellerPartnerId);

            return Ok(prices);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] PriceMaterialPartnerUpdateDto dto)
        {
            await _service.UpdatePriceAsync(dto);
            return NoContent();
        }
    }
}
