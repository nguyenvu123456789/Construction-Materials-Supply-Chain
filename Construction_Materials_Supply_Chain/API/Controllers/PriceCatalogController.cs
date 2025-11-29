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

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] PriceMaterialPartnerUpdateDto dto)
        {
            await _service.UpdatePriceAsync(dto);
            return NoContent();
        }
    }
}
