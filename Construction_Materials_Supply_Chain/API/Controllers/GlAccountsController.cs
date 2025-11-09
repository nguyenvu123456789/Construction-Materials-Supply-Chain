using Application.DTOs;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/gl-accounts")]
    public class GlAccountsController : ControllerBase
    {
        private readonly IGlAccountService _svc;
        public GlAccountsController(IGlAccountService svc) { _svc = svc; }

        [HttpGet]
        public IActionResult List([FromQuery] int partnerId, [FromQuery] string? q, [FromQuery] bool includeDeleted = false)
            => Ok(_svc.List(partnerId, q, includeDeleted));

        [HttpGet("{id:int}")]
        public IActionResult Get(int id, [FromQuery] bool includeDeleted = false)
        {
            var dto = _svc.Get(id, includeDeleted);
            if (dto == null) return NotFound();
            return Ok(dto);
        }

        [HttpPost]
        public IActionResult Create([FromBody] GlAccountCreateDto dto)
        {
            var res = _svc.Create(dto);
            if (!res.ok) return BadRequest(new { error = res.error });
            return Ok(res.data);
        }

        [HttpPut("{id:int}")]
        public IActionResult Update(int id, [FromBody] GlAccountUpdateDto dto)
        {
            var res = _svc.Update(id, dto);
            if (!res.ok) return BadRequest(new { error = res.error });
            return Ok(res.data);
        }

        [HttpDelete("{id:int}")]
        public IActionResult SoftDelete(int id)
        {
            var res = _svc.SoftDelete(id);
            if (!res.ok) return NotFound(new { error = res.error });
            return NoContent();
        }

        [HttpPost("{id:int}/restore")]
        public IActionResult Restore(int id)
        {
            var res = _svc.Restore(id);
            if (!res.ok) return BadRequest(new { error = res.error });
            return NoContent();
        }
    }
}
