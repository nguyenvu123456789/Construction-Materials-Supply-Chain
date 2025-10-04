using API.DTOs;
using API.Helper.Paging;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActivityLogsController : ControllerBase
    {
        private readonly IActivityLogService _service;
        private readonly IMapper _mapper;

        public ActivityLogsController(IActivityLogService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<ActivityLogDto>> GetLogs()
        {
            var logs = _service.GetAll();
            return Ok(_mapper.Map<IEnumerable<ActivityLogDto>>(logs));
        }

        [HttpGet("filter")]
        public ActionResult<PagedResultDto<ActivityLogDto>> GetLogsFiltered([FromQuery] ActivityLogPagedQueryDto queryParams)
        {
            var logs = _service.GetFiltered(queryParams.SearchTerm, queryParams.FromDate, queryParams.ToDate, queryParams.PageNumber, queryParams.PageSize, out var totalCount);

            var result = new PagedResultDto<ActivityLogDto>
            {
                Data = _mapper.Map<IEnumerable<ActivityLogDto>>(logs),
                TotalCount = totalCount,
                PageNumber = queryParams.PageNumber,
                PageSize = queryParams.PageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)queryParams.PageSize)
            };

            return Ok(result);
        }
    }
}
