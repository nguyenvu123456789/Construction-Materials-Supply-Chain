using Application.DTOs;
using AutoMapper;
using Domain.Models;
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
                return BadRequest("Invalid report data.");

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
                return Ok(new { message = "Report reviewed successfully." });
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
            var report = _reportService.GetById(reportId);
            if (report == null) return NotFound();
            return Ok(_mapper.Map<ExportReportResponseDto>(report));
        }

        // 🔹 GET: /api/ExportReports/pending
        [HttpGet("pending")]
        public IActionResult GetPendingReports()
        {
            var reports = _reportService.GetAllPending();
            return Ok(_mapper.Map<IEnumerable<ExportReportResponseDto>>(reports));
        }
        // 🔹 GET: /api/ExportReports/reviewed
        [HttpGet("reviewed")]
        public IActionResult GetReviewedReports()
        {
            var reports = _reportService.GetAllReviewed();
            return Ok(_mapper.Map<IEnumerable<ExportReportResponseDto>>(reports));
        }

    }
}
