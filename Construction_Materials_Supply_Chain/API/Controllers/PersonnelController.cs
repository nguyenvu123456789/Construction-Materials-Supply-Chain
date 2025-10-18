using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/personnel")]
    public class PersonnelController : ControllerBase
    {
        private readonly IPersonnelService _svc;
        public PersonnelController(IPersonnelService svc) { _svc = svc; }

        [HttpPost("{type:regex(^driver|porter|vehicle$)}")]
        public ActionResult<PersonResponseDto> Create([FromRoute] string type, [FromBody] PersonCreateDto dto)
        { dto.Type = type; return Ok(_svc.Create(dto)); }

        [HttpGet("{type:regex(^driver|porter|vehicle$)}/{id:int}")]
        public ActionResult<PersonResponseDto> Get([FromRoute] string type, int id)
        { var x = _svc.Get(type, id); return x == null ? NotFound() : Ok(x); }

        [HttpGet("{type:regex(^driver|porter|vehicle$)}")]
        public ActionResult<IEnumerable<PersonResponseDto>> GetAll([FromRoute] string type)
        => Ok(_svc.GetAll(type));

        [HttpGet("{type:regex(^driver|porter|vehicle$)}/search")]
        public ActionResult<IEnumerable<PersonResponseDto>> Search([FromRoute] string type, [FromQuery] string? q, [FromQuery] bool? active, [FromQuery] int? top)
        => Ok(_svc.Search(type, q, active, top));

        [HttpPut("{type:regex(^driver|porter|vehicle$)}/{id:int}")]
        public IActionResult Update([FromRoute] string type, int id, [FromBody] PersonUpdateDto dto)
        { _svc.Update(type, id, dto); return Ok(); }

        [HttpDelete("{type:regex(^driver|porter|vehicle$)}/{id:int}")]
        public IActionResult Delete([FromRoute] string type, int id)
        { _svc.Delete(type, id); return Ok(); }
    }
}
