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

        // 🔹 Tạo báo cáo vật tư hỏng
        [HttpPost]
        public IActionResult CreateReport([FromBody] CreateExportReportDto dto)
        {
            if (dto == null)
                return BadRequest("Invalid request data");

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

        // 🔹 Quản lý duyệt báo cáo
        [HttpPost("{reportId}/review")]
        public IActionResult ReviewReport(int reportId, [FromBody] ReviewExportReportDto dto)
        {
            try
            {
                _reportService.ReviewReport(reportId, dto);
                return Ok(new { message = "Report reviewed successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // 🔹 Lấy chi tiết báo cáo theo ID
        [HttpGet("{reportId:int}")]
        public IActionResult GetReport(int reportId)
        {
            var report = _reportService.GetById(reportId);
            if (report == null) return NotFound();
            return Ok(_mapper.Map<ExportReportResponseDto>(report));
        }

        // 🔹 Lấy danh sách các báo cáo Pending
        [HttpGet("pending")]
        public IActionResult GetAllPending()
        {
            var reports = _reportService.GetAllPending();
            return Ok(_mapper.Map<IEnumerable<ExportReportResponseDto>>(reports));
        }
    }
}
