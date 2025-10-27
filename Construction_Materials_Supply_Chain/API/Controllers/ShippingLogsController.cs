using Application.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShippingLogsController : ControllerBase
    {
        private readonly IShippingLogService _service;
        private readonly IMapper _mapper;

        public ShippingLogsController(IShippingLogService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        //[HttpGet]
        //public ActionResult<IEnumerable<ShippingLogDto>> GetAllShippingLogs()
        //{
        //    var logs = _service.GetAll();
        //    return Ok(_mapper.Map<IEnumerable<ShippingLogDto>>(logs));
        //}
    }
}
