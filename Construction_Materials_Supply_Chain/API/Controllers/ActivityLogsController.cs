using API.DTOs;
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
        public ActionResult<object> GetLogsFiltered([FromQuery] QueryParametersDto queryParams)
        {
            var logs = _repository.GetLogsPaged(queryParams.Keyword, queryParams.PageNumber, queryParams.PageSize);
            var totalCount = _repository.GetTotalLogsCount(queryParams.Keyword);

            var result = new
            {
                Data = _mapper.Map<IEnumerable<ActivityLogDto>>(logs),
                TotalCount = totalCount,
                queryParams.PageNumber,
                queryParams.PageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)queryParams.PageSize)
            };

            return Ok(result);
        }
    }
}
