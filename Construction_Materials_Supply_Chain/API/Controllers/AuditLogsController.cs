using API.DTOs;
using Application.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuditLogsController : ControllerBase
    {
        private readonly IAuditLogService _service;
        private readonly IMapper _mapper;

        public AuditLogsController(IAuditLogService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        // GET: api/auditlogs?Keyword=...&PageNumber=1&PageSize=20
        [HttpGet]
        public ActionResult<object> GetAuditLogs([FromQuery] QueryParametersDto query)
        {
            var logs = _service.GetFiltered(query.Keyword, query.PageNumber, query.PageSize, out var total);
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
