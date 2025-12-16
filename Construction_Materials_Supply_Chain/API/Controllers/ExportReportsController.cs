using Application.Constants.Messages;
using Application.DTOs;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExportReportsController : ControllerBase
    {
        private readonly IExportReportService _reportService;
        private readonly IMapper _mapper;

        public ExportReportsController(IExportReportService reportService, IMapper mapper)
        {
            _reportService = reportService;
            _mapper = mapper;
        }

        // 🔹 POST: /api/ExportReports
        [HttpPost]
        public IActionResult CreateReport([FromBody] CreateExportReportDto dto)
        {
            if (dto == null || dto.Details.Count == 0)
                return BadRequest(new { message = ExportMessages.MSG_REQUIRE_AT_LEAST_ONE_MATERIAL });

            try
            {
                var report = _reportService.CreateReport(dto);
                return Ok(_mapper.Map<ExportReportResponseDto>(report));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // 🔹 POST: /api/ExportReports/{id}/review
        [HttpPost("{reportId}/review")]
        public IActionResult ReviewReport(int reportId, [FromBody] ReviewExportReportDto dto)
        {
            try
            {
                _reportService.ReviewReport(reportId, dto);
                return Ok(new { message = ExportMessages.MSG_EXPORT_APPROVED });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // 🔹 GET: /api/ExportReports/{id}
        [HttpGet("{reportId:int}")]
        public IActionResult GetReport(int reportId)
        {
            try
            {
                var report = _reportService.GetById(reportId);
                if (report == null)
                    return NotFound(new { message = ExportMessages.EXPORT_NOT_FOUND });

                return Ok(_mapper.Map<ExportReportResponseDto>(report));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("reports")]
        public IActionResult GetReports([FromQuery] int? partnerId = null, [FromQuery] int? createdByUserId = null)
        {
            try
            {
                var reports = _reportService.GetAllReports(partnerId, createdByUserId);
                var reportDtos = _mapper.Map<List<ExportReportResponseDto>>(reports);

                return Ok(reportDtos);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // 🔹 PUT: /api/ExportReports/{reportId}/view
        [HttpPut("{reportId:int}/view")]
        public IActionResult MarkAsViewed(int reportId)
        {
            try
            {
                _reportService.MarkAsViewed(reportId);
                return Ok(new { message = ExportMessages.MSG_MARK_VIEWED });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
