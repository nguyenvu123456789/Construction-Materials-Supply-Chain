using API.DTOs;
using API.Helper.Paging;
using Application.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PartnersController : ControllerBase
    {
        private readonly IPartnerService _service;
        private readonly IMapper _mapper;

        public PartnersController(IPartnerService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        // GET: api/partners/grouped-by-type
        [HttpGet("grouped-by-type")]
        public ActionResult<IEnumerable<PartnerTypeDto>> GetPartnersGroupedByType()
        {
            var types = _service.GetPartnerTypesWithPartners();
            var result = types.Select(pt => new PartnerTypeDto
            {
                PartnerTypeId = pt.PartnerTypeId,
                TypeName = pt.TypeName,
                Partners = pt.Partners.Select(p => _mapper.Map<PartnerDto>(p)).ToList()
            }).ToList();

            return Ok(result);
        }

        // GET: api/partners/by-type/1
        [HttpGet("by-type/{partnerTypeId:int}")]
        public ActionResult<IEnumerable<PartnerDto>> GetPartnersByType(int partnerTypeId)
        {
            var partners = _service.GetPartnersByType(partnerTypeId);
            return Ok(_mapper.Map<IEnumerable<PartnerDto>>(partners));
        }

        // GET: api/partners/filter
        [HttpGet("filter")]
        public ActionResult<PagedResultDto<PartnerDto>> GetPartnersFiltered([FromQuery] PartnerPagedQueryDto queryParams)
        {
            var partners = _service.GetPartnersFiltered(queryParams.SearchTerm, queryParams.PartnerTypes, queryParams.PageNumber, queryParams.PageSize, out var totalCount);

            var result = new PagedResultDto<PartnerDto>
            {
                Data = _mapper.Map<IEnumerable<PartnerDto>>(partners),
                TotalCount = totalCount,
                PageNumber = queryParams.PageNumber,
                PageSize = queryParams.PageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)queryParams.PageSize)
            };

            return Ok(result);
        }
    }
}
