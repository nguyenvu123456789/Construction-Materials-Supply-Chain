using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaterialsController : ControllerBase
    {
        private readonly IMaterialService _service;

        public MaterialsController(IMaterialService service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var materials = _service.GetAll();
            return Ok(materials);
        }

        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            var material = _service.GetById(id);
            if (material == null) return NotFound();
            return Ok(material);
        }

        [HttpPost]
        public IActionResult Create(Material material)
        {
            _service.Create(material);
            return CreatedAtAction(nameof(GetById), new { id = material.MaterialId }, material);
        }

        [HttpPut("{id:int}")]
        public IActionResult Update(int id, Material material)
        {
            if (id != material.MaterialId) return BadRequest();
            var existing = _service.GetById(id);
            if (existing == null) return NotFound();

            _service.Update(material);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            var existing = _service.GetById(id);
            if (existing == null) return NotFound();

            _service.Delete(id);
            return NoContent();
        }
    }
}
