using Application.DTOs;
using Domain.Interface;
using Domain.Models;

namespace Application.Services.Implements
{
    public class ExportReportService : IExportReportService
    {
        private readonly IExportReportRepository _reportRepo;
        private readonly IExportRepository _exportRepo;
        private readonly IInventoryRepository _inventoryRepo;

        public ExportReportService(
            IExportReportRepository reportRepo,
            IExportRepository exportRepo,
            IInventoryRepository inventoryRepo)
        {
            _reportRepo = reportRepo;
            _exportRepo = exportRepo;
            _inventoryRepo = inventoryRepo;
        }

        // 🔹 Nhân viên tạo báo cáo hư hỏng (chưa có quyết định giữ/lỗi)
        public ExportReport CreateReport(CreateExportReportDto dto)
        {
            var export = _exportRepo.GetById(dto.ExportId)
                         ?? throw new Exception("Không tìm thấy phiếu xuất.");

            var report = new ExportReport
            {
                ExportId = export.ExportId,
                ReportedBy = dto.ReportedBy,
                Notes = dto.Notes,
                Status = "Pending",
                ReportDate = DateTime.UtcNow
            };

            foreach (var d in dto.Details)
            {
                var detail = new ExportReportDetail
                {
                    MaterialId = d.MaterialId,
                    QuantityDamaged = d.QuantityDamaged,
                    Reason = d.Reason,
                    Keep = null 
                };
                report.ExportReportDetails.Add(detail);
            }

            _reportRepo.Add(report);
            return report;
        }

        // 🔹 Quản lý duyệt báo cáo
        public void ReviewReport(int reportId, ReviewExportReportDto dto)
        {
            var report = _reportRepo.GetByIdWithDetails(reportId) // dùng method có include detail
                         ?? throw new Exception("Không tìm thấy báo cáo hư hỏng.");

            var export = _exportRepo.GetById(report.ExportId)
                         ?? throw new Exception("Không tìm thấy phiếu xuất.");

            var warehouseId = export.WarehouseId;

            report.DecidedBy = dto.DecidedBy;
            report.DecidedAt = DateTime.UtcNow;
            report.Notes = dto.Notes;

            if (dto.Approve == null)
                throw new Exception("Phải chọn duyệt hoặc từ chối báo cáo.");

            if (dto.Approve == false)
            {
                report.Status = "Rejected";
                _reportRepo.Update(report);
                return;
            }

            if (dto.Details == null || !dto.Details.Any())
                throw new Exception("Thiếu danh sách chi tiết duyệt vật tư.");

            foreach (var d in report.ExportReportDetails)
            {
                var decisionDetail = dto.Details.FirstOrDefault(x => x.MaterialId == d.MaterialId);
                if (decisionDetail == null)
                    throw new Exception($"Thiếu quyết định cho vật tư ID {d.MaterialId}.");

                bool keep = decisionDetail.Keep;

                // ❌ Cập nhật quyết định vào chi tiết
                d.Keep = keep;

                // ✅ Trừ kho nếu vật tư hư hỏng
                if (!keep)
                {
                    var inventory = _inventoryRepo.GetByMaterialId(d.MaterialId, warehouseId)
                        ?? throw new Exception($"Không tìm thấy vật tư {d.MaterialId} trong kho {warehouseId}.");

                    decimal currentQty = inventory.Quantity ?? 0;

                    if (currentQty < d.QuantityDamaged)
                        throw new Exception($"Không đủ vật tư {d.MaterialId} trong kho {warehouseId}.");

                    inventory.Quantity = currentQty - d.QuantityDamaged;
                    _inventoryRepo.Update(inventory);
                }
            }

            // ✅ Bắt buộc EF Core update các entity con
            _reportRepo.Update(report); // update cả report và detail
        }


        // 🔹 Lấy báo cáo theo ID
        public ExportReport? GetById(int reportId) =>
            _reportRepo.GetById(reportId);

        // 🔹 Lấy danh sách báo cáo chờ duyệt
        public List<ExportReport> GetAllPending() =>
            _reportRepo.GetAll().Where(r => r.Status == "Pending").ToList();
    }
}
