using API.DTOs;
using API.Helper.Paging;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Repositories.Interface;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActivityLogsController : ControllerBase
    {
        private readonly IActivityLogRepository _repository;
        private readonly IMapper _mapper;

        public ActivityLogsController(IActivityLogRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<ActivityLogDto>> GetLogs()
        {
            var logs = _repository.GetLogs();
            return Ok(_mapper.Map<IEnumerable<ActivityLogDto>>(logs));
        }

        [HttpGet("filter")]
        public ActionResult<PagedResultDto<ActivityLogDto>> GetLogsFiltered([FromQuery] ActivityLogPagedQueryDto queryParams)
        {
            var logs = _repository.GetLogsPaged(queryParams.SearchTerm, queryParams.FromDate, queryParams.ToDate, queryParams.PageNumber, queryParams.PageSize);
            var totalCount = _repository.GetTotalLogsCount(queryParams.SearchTerm, queryParams.FromDate, queryParams.ToDate);

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