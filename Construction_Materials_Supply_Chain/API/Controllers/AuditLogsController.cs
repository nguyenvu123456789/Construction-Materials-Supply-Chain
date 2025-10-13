using Application.Common.Pagination;
using Application.DTOs;
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

        [HttpGet]
        public ActionResult<PagedResultDto<AuditLogDto>> GetAuditLogs([FromQuery] PagedQueryDto query)
        {
            var logs = _service.GetFiltered(query.SearchTerm, query.PageNumber, query.PageSize, out var totalCount);
            var dtoList = _mapper.Map<IEnumerable<AuditLogDto>>(logs);
            var totalPages = (int)Math.Ceiling((double)totalCount / query.PageSize);

            var result = new PagedResultDto<AuditLogDto>
            {
                Data = dtoList,
                TotalCount = totalCount,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize,
                TotalPages = totalPages
            };

            return Ok(result);
        }
    }
}
