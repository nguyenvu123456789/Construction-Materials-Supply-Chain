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

        [HttpGet("Search")]
        public ActionResult<IEnumerable<ActivityLogDto>> SearchLogs(string keyword)
        {
            var logs = _repository.SearchLogs(keyword);
            return Ok(_mapper.Map<IEnumerable<ActivityLogDto>>(logs));
        }
    }
}
