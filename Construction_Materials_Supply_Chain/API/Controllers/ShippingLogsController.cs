using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ShippingLogsController : ControllerBase
    {
        private readonly IShippingLogService _service;
        private readonly IMapper _mapper;

        public ShippingLogsController(IShippingLogService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Staff")]
        public ActionResult<IEnumerable<ShippingLogDto>> GetAllShippingLogs()
        {
            var logs = _service.GetAll();
            return Ok(_mapper.Map<IEnumerable<ShippingLogDto>>(logs));
        }
    }
}
