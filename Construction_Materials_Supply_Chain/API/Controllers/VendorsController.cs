using API.DTOs;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Repositories.Interface;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VendorsController : ControllerBase
    {
        private readonly IVendorRepository _repository;
        private readonly IMapper _mapper;

        public VendorsController(IVendorRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        // GET: api/Vendors
        [HttpGet]
        public ActionResult<IEnumerable<VendorDto>> GetApprovedVendors()
        {
            var vendors = _repository.GetApprovedVendors();
            return Ok(_mapper.Map<IEnumerable<VendorDto>>(vendors));
        }

        // GET: api/Vendors/Search?keyword=abc
        [HttpGet("Search")]
        public ActionResult<IEnumerable<VendorDto>> Search(string keyword)
        {
            var vendors = _repository.SearchVendors(keyword);
            return Ok(_mapper.Map<IEnumerable<VendorDto>>(vendors));
        }
    }
}
