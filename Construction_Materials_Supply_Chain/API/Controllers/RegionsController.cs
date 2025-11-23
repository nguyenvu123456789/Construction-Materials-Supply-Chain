using Application.DTOs;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/regions")]
    public class RegionsController : ControllerBase
    {
        private readonly IRegionService _service;

        public RegionsController(IRegionService service)
        {
            _service = service;
        }

        [HttpGet]
        public ActionResult<IEnumerable<RegionDto>> GetAll()
            => Ok(_service.GetAll());

        [HttpGet("{id:int}")]
        public ActionResult<RegionDto> GetById(int id)
        {
            var region = _service.GetById(id);
            if (region == null) return NotFound();
            return Ok(region);
        }

        [HttpPost]
        public ActionResult<RegionDto> Create([FromBody] RegionCreateDto dto)
        {
            var created = _service.Create(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.RegionId }, created);
        }

        [HttpPut("{id:int}")]
        public IActionResult Update(int id, [FromBody] RegionUpdateDto dto)
        {
            _service.Update(id, dto);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            _service.Delete(id);
            return NoContent();
        }
    }
}
