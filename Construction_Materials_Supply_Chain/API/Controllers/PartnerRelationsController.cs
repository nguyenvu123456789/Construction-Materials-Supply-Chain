using Application.DTOs.Relations;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PartnerRelationsController : ControllerBase
    {
        private readonly IPartnerRelationService _service;

        public PartnerRelationsController(IPartnerRelationService service)
        {
            _service = service;
        }

        // GET: api/partner-relations
        [HttpGet]
        public IActionResult GetAll()
        {
            var data = _service.GetAll();
            return Ok(data);
        }

        // GET: api/partner-relations/5
        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            var item = _service.GetById(id);
            if (item == null)
                return NotFound(new { message = "Relation not found" });

            return Ok(item);
        }

        // POST: api/partner-relations
        [HttpPost]
        public IActionResult Create([FromBody] PartnerRelationCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = _service.Create(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.PartnerRelationId }, created);
        }

        // PUT: api/partner-relations/5
        [HttpPut("{id:int}")]
        public IActionResult Update(int id, [FromBody] PartnerRelationUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                _service.Update(id, dto);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Relation not found" });
            }

            return NoContent();
        }

        // DELETE: api/partner-relations/5
        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            _service.Delete(id);
            return NoContent();
        }
        // GET: api/partner-relations/by-buyer/5
        [HttpGet("by-buyer/{buyerId:int}")]
        public IActionResult GetByBuyer(int buyerId)
        {
            var data = _service.GetByBuyer(buyerId);
            return Ok(data);
        }

        // GET: api/partner-relations/by-seller/5
        [HttpGet("by-seller/{sellerId:int}")]
        public IActionResult GetBySeller(int sellerId)
        {
            var data = _service.GetBySeller(sellerId);
            return Ok(data);
        }

    }
}
