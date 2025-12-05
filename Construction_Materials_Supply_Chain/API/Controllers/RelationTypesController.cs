using Application.DTOs.RelationType;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RelationTypesController : ControllerBase
    {
        private readonly IRelationTypeService _service;

        public RelationTypesController(IRelationTypeService service)
        {
            _service = service;
        }

        [HttpGet("partner/{partnerId:int}")]
        public IActionResult GetByPartner(int partnerId, int pageNumber = 1, int pageSize = 10)
        {
            return Ok(_service.GetByPartner(partnerId, pageNumber, pageSize));
        }

        [HttpPut("{id:int}")]
        public IActionResult Update(int id, RelationTypeDto dto)
        {
            _service.Update(id, dto);
            return Ok("Updated");
        }

    }
}
