using API.DTOs;
using AutoMapper;
using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Repositories.Interface;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImportReportsController : ControllerBase
    {
        private readonly IImportReportRepository _reportRepository;
        private readonly IImportRepository _importRepository;
        private readonly IMapper _mapper;

        public ImportReportsController(
            IImportReportRepository reportRepository,
            IImportRepository importRepository,
            IMapper mapper)
        {
            _reportRepository = reportRepository;
            _importRepository = importRepository;
            _mapper = mapper;
        }

        // 🟢 1. Tạo báo cáo nhập thiếu
        [HttpPost]
        public IActionResult CreateImportReport([FromBody] ImportReportCreateDto dto)
        {
            var import = _importRepository.GetPendingInvoiceByCode(""); // placeholder
            import = null; // tránh cảnh báo unused, bạn có thể bỏ dòng này

            if (dto == null || dto.Details.Count == 0)
                return BadRequest("Dữ liệu báo cáo không hợp lệ.");

            var report = new ImportReport
            {
                ImportId = dto.ImportId,
                CreatedBy = dto.CreatedBy,
                Notes = dto.Notes,
                CreatedAt = DateTime.Now,
                Status = "Pending",
                ImportReportDetails = dto.Details.Select(d => new ImportReportDetail
                {
                    MaterialId = d.MaterialId,
                    TotalQuantity = d.TotalQuantity,
                    GoodQuantity = d.GoodQuantity,
                    DamagedQuantity = d.DamagedQuantity,
                    Comment = d.Comment
                }).ToList()
            };

            _reportRepository.CreateImportReport(report);
            return Ok(new { Message = "Đã gửi báo cáo nhập thiếu thành công", ReportId = report.ImportReportId });
        }

        // 🟡 2. Quản lý duyệt hoặc từ chối
        [HttpPut("review")]
        public IActionResult ReviewImportReport([FromBody] ImportReportReviewDto dto)
        {
            var report = _reportRepository.GetImportReportById(dto.ReportId);
            if (report == null) return NotFound("Không tìm thấy báo cáo.");

            if (report.Status != "Pending")
                return BadRequest("Báo cáo đã được duyệt hoặc từ chối trước đó.");

            report.Status = dto.Approved ? "Approved" : "Rejected";
            report.ReviewedBy = dto.ReviewedBy;
            report.ReviewedAt = DateTime.Now;
            report.RejectReason = dto.Approved ? null : dto.RejectReason;

            _reportRepository.UpdateImportReport(report);
            return Ok(new { Message = $"Báo cáo đã được {(dto.Approved ? "duyệt" : "từ chối")}" });
        }

        [HttpGet]
        public IActionResult GetImportReports()
        {
            var reports = _reportRepository.GetImportReports();
            return Ok(reports);
        }

        [HttpGet("{id}")]
        public IActionResult GetImportReport(int id)
        {
            var report = _reportRepository.GetImportReportById(id);
            if (report == null)
                return NotFound("Không tìm thấy báo cáo.");

            return Ok(report);
        }
    }
}
