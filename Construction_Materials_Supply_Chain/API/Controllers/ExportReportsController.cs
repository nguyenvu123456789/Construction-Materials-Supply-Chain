using API.DTOs;
using AutoMapper;
using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Repositories.Interface;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExportReportsController : ControllerBase
    {
        private readonly IExportReportRepository _exportReportRepo;
        private readonly IInventoryRepository _inventoryRepo;
        private readonly IMapper _mapper;

        public ExportReportsController(
            IExportReportRepository exportReportRepo,
            IInventoryRepository inventoryRepo,
            IMapper mapper)
        {
            _exportReportRepo = exportReportRepo;
            _inventoryRepo = inventoryRepo;
            _mapper = mapper;
        }

        // 🧾 1️⃣ Thủ kho tạo báo cáo
        [HttpPost]
        public IActionResult CreateReport([FromBody] ExportReportCreateDto dto)
        {
            if (dto == null || dto.Details == null || dto.Details.Count == 0)
                return BadRequest("Báo cáo không có chi tiết vật tư.");

            var report = new ExportReport
            {
                ExportId = dto.ExportId,
                ReportedBy = dto.ReportedBy,
                Notes = dto.Notes,
                Status = "Pending",
                ReportDate = DateTime.Now,
                ExportReportDetails = dto.Details.Select(d => new ExportReportDetail
                {
                    MaterialId = d.MaterialId,
                    Quantity = d.Quantity,
                    Reason = d.Reason,
                    Keep = d.Keep
                }).ToList()
            };

            _exportReportRepo.CreateReport(report);

            return Ok(new
            {
                message = "Tạo báo cáo xuất kho thành công.",
                reportId = report.ExportReportId
            });
        }

        // 🧾 2️⃣ Quản lý duyệt báo cáo
        [HttpPut("{id}/approve")]
        public IActionResult ApproveReport(int id, [FromQuery] int decidedBy)
        {
            var report = _exportReportRepo.GetReportById(id);
            if (report == null)
                return NotFound("Không tìm thấy báo cáo.");

            report.Status = "Approved";
            report.DecidedAt = DateTime.Now;
            report.DecidedBy = decidedBy;

            // ✅ Trừ hàng hư (Keep = false)
            foreach (var detail in report.ExportReportDetails)
            {
                if (!detail.Keep)
                {
                    _inventoryRepo.DecreaseQuantity(
                        report.Export.WarehouseId,
                        detail.MaterialId,
                        detail.Quantity
                    );
                }
            }

            _exportReportRepo.UpdateReport(report);
            return Ok("Báo cáo đã được duyệt và cập nhật tồn kho.");
        }

        // 🧾 3️⃣ Quản lý từ chối báo cáo
        [HttpPut("{id}/reject")]
        public IActionResult RejectReport(int id, [FromQuery] int decidedBy, [FromBody] string reason)
        {
            var report = _exportReportRepo.GetReportById(id);
            if (report == null)
                return NotFound("Không tìm thấy báo cáo.");

            report.Status = "Rejected";
            report.DecidedAt = DateTime.Now;
            report.DecidedBy = decidedBy;
            report.Notes = reason;

            _exportReportRepo.UpdateReport(report);
            return Ok("Báo cáo đã bị từ chối.");
        }

        // 🧾 4️⃣ Xem chi tiết báo cáo
        [HttpGet("{id}")]
        public IActionResult GetReport(int id)
        {
            var report = _exportReportRepo.GetReportById(id);
            if (report == null)
                return NotFound("Không tìm thấy báo cáo.");

            return Ok(report);
        }
    }
}
