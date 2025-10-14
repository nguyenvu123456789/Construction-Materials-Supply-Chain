using Application.Common.Pagination;
using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PartnersController : ControllerBase
    {
        private readonly IPartnerService _service;
        public PartnersController(IPartnerService service) => _service = service;

        [HttpGet]
        public ActionResult<IEnumerable<PartnerDto>> GetAll() => Ok(_service.GetAllDto());

        [HttpGet("{id:int}")]
        public ActionResult<PartnerDto> GetById(int id)
        {
            var dto = _service.GetDto(id);
            return dto == null ? NotFound() : Ok(dto);
        }

        [HttpPost]
        public ActionResult<PartnerDto> Create([FromBody] PartnerCreateDto dto)
        {
            var created = _service.Create(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.PartnerId }, created);
        }

        [HttpPut("{id:int}")]
        public IActionResult Update(int id, [FromBody] PartnerUpdateDto dto)
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

        [HttpPost("{id:int}/restore")]
        public IActionResult Restore(int id, [FromQuery] string status = "Active")
        {
            _service.Restore(id, status);
            return NoContent();
        }

        [HttpGet("grouped-by-type")]
        public ActionResult<IEnumerable<PartnerTypeDto>> GroupedByType() => Ok(_service.GetPartnerTypesWithPartnersDto());

        [HttpGet("by-type/{partnerTypeId:int}")]
        public ActionResult<IEnumerable<PartnerDto>> ByType(int partnerTypeId) => Ok(_service.GetPartnersByTypeDto(partnerTypeId));

        [HttpGet("filter")]
        public ActionResult<PagedResultDto<PartnerDto>> Filter([FromQuery] PartnerPagedQueryDto query, [FromQuery] List<string>? statuses)
            => Ok(_service.GetPartnersFiltered(query, statuses));

        [HttpGet("filter-all")]
        public ActionResult<PagedResultDto<PartnerDto>> FilterAll([FromQuery] PartnerPagedQueryDto query, [FromQuery] List<string>? statuses)
            => Ok(_service.GetPartnersFilteredIncludeDeleted(query, statuses));

        [HttpGet("types")]
        public ActionResult<IEnumerable<PartnerTypeDto>> Types() => Ok(_service.GetPartnerTypesDto());
    }
}
