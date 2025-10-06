using API.DTOs;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Repositories.Interface;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuditLogsController : ControllerBase
    {
        private readonly IAuditLogRepository _repository;
        private readonly IMapper _mapper;

        public AuditLogsController(IAuditLogRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        // GET: api/auditlogs
        [HttpGet]
        public ActionResult<object> GetAuditLogs([FromQuery] QueryParametersDto query)
        {
            var logs = _repository.GetAuditLogs(query.Keyword, query.PageNumber, query.PageSize);
            var total = _repository.CountAuditLogs(query.Keyword);

            return Ok(new
            {
                Data = _mapper.Map<IEnumerable<AuditLogDto>>(logs),
                TotalCount = total,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize
            });
        }
    }
}
