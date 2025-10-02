using API.DTOs;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Repositories.Interface;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupplyChainController : ControllerBase
    {
        private readonly ISupplyChainRepository _repository;
        private readonly IMapper _mapper;

        public SupplyChainController(ISupplyChainRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        // GET: api/supplychain/partners
        [HttpGet("partners")]
        public ActionResult<IEnumerable<PartnerTypeDto>> GetPartnersGroupedByType()
        {
            var partnerTypes = _repository.GetPartnerTypes();

            var result = partnerTypes.Select(pt => new PartnerTypeDto
            {
                PartnerTypeId = pt.PartnerTypeId,
                TypeName = pt.TypeName,
                Partners = pt.Partners.Select(p => _mapper.Map<PartnerDto>(p)).ToList()
            }).ToList();

            return Ok(result);
        }

        [HttpGet("partners/by-type/{partnerTypeId}")]
        public ActionResult<IEnumerable<PartnerDto>> GetPartnersByType(int partnerTypeId)
        {
            var partnerTypes = _repository.GetPartnerTypes();
            var partnerType = partnerTypes.FirstOrDefault(pt => pt.PartnerTypeId == partnerTypeId);

            if (partnerType == null)
            {
                return NotFound(new { Message = "PartnerType not found" });
            }

            var dto = _mapper.Map<IEnumerable<PartnerDto>>(partnerType.Partners);
            return Ok(dto);
        }
    }
}
