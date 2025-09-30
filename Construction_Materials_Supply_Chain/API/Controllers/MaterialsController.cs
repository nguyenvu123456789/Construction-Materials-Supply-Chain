using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Repositories.Interface;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaterialsController : ControllerBase
    {
        private readonly IMaterialRepository _repository;

        public MaterialsController(IMaterialRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var materials = _repository.GetMaterials();
            return Ok(materials);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var material = _repository.GetMaterialById(id);
            if (material == null) return NotFound();
            return Ok(material);
        }

        [HttpPost]
        public IActionResult Create(Material material)
        {
            _repository.AddMaterial(material);
            return CreatedAtAction(nameof(GetById), new { id = material.MaterialId }, material);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, Material material)
        {
            if (id != material.MaterialId) return BadRequest();
            _repository.UpdateMaterial(material);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _repository.DeleteMaterial(id);
            return NoContent();
        }
    }
}
