using API.DTOs;
using AutoMapper;
using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Repositories.Interface;
using Repositories.Repositories;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShippingLogsController : ControllerBase
    {
        private readonly IShippingLogRepository _repository;
        private readonly IMapper _mapper;

        public ShippingLogsController(IShippingLogRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<ShippingLogDto>> GetAllShippingLogs()
        {
            var logs = _repository.GetAllShippingLogs();
            return Ok(_mapper.Map<IEnumerable<ShippingLogDto>>(logs));
        }

        [HttpGet("Search")]
        public ActionResult<IEnumerable<ShippingLogDto>> Search(string status)
        {
            var logs = _repository.SearchShippingLogs(status);
            return Ok(_mapper.Map<IEnumerable<ShippingLogDto>>(logs));
        }
    }
}
