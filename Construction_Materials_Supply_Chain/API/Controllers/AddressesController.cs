using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/addresses")]
    public class AddressesController : ControllerBase
    {
        private readonly IAddressService _service;
        public AddressesController(IAddressService svc) { _service = svc; }

        [HttpPost]
        public ActionResult<AddressResponseDto> Create([FromBody] AddressCreateDto dto) => Ok(_service.Create(dto));

        [HttpGet("{id:int}")]
        public ActionResult<AddressResponseDto> Get(int id)
        {
            var x = _service.Get(id);
            return x == null ? NotFound() : Ok(x);
        }

        [HttpGet]
        public ActionResult<IEnumerable<AddressResponseDto>> GetAll() => Ok(_service.GetAll());

        [HttpGet("search")]
        public ActionResult<IEnumerable<AddressResponseDto>> Search([FromQuery] string? q, [FromQuery] string? city, [FromQuery] int? top)
            => Ok(_service.Search(q, city, top));

        [HttpPut("{id:int}")]
        public IActionResult Update(int id, [FromBody] AddressUpdateDto dto) { _service.Update(id, dto); return Ok(); }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id) { _service.Delete(id); return Ok(); }
    }
}
