using Application.Common.Pagination;
using Application.DTOs.Application.DTOs;
using Application.DTOs.Common.Pagination;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/price-catalog")]
    public class PriceCatalogController : ControllerBase
    {
        private readonly IPriceMaterialPartnerService _service;
        public PriceCatalogController(IPriceMaterialPartnerService service) { _service = service; }

        [HttpGet]
        public ActionResult<PagedResultDto<PriceMaterialPartnerDto>> Get([FromQuery] PriceCatalogQueryDto q)
        {
            var res = _service.GetAll(q);
            return Ok(res);
        }

        [HttpPut]
        public IActionResult Update([FromBody] PriceMaterialPartnerUpdateDto dto)
        {
            _service.UpdatePrice(dto);
            return NoContent();
        }
    }
}