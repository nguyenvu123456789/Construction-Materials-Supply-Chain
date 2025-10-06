using Application.Common.Pagination;
using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActivityLogsController : ControllerBase
    {
        private readonly IActivityLogService _service;

        public ActivityLogsController(IActivityLogService service)
        {
            _service = service;
        }

        [HttpGet]
        public ActionResult<IEnumerable<ActivityLogDto>> GetLogs()
            => Ok(_service.GetAllDto());

        [HttpGet("filter")]
        public ActionResult<PagedResultDto<ActivityLogDto>> GetLogsFiltered([FromQuery] ActivityLogPagedQueryDto query)
            => Ok(_service.GetFiltered(query));
    }
}
