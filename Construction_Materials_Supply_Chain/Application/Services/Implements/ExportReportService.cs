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
        private readonly IHandleRequestRepository _handleRequests;
        private readonly IUserRepository _userRepo;

        public ExportReportService(
            IExportReportRepository reportRepo,
            IExportRepository exportRepo,
            IInventoryRepository inventoryRepo,
            IHandleRequestRepository handleRequests,
            IUserRepository userRepo)
        {
            _reportRepo = reportRepo;
            _exportRepo = exportRepo;
            _inventoryRepo = inventoryRepo;
            _handleRequests = handleRequests;
            _userRepo = userRepo;
        }

        // 🔹 Tạo báo cáo hư hỏng
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
                report.ExportReportDetails.Add(new ExportReportDetail
                {
                    MaterialId = d.MaterialId,
                    QuantityDamaged = d.QuantityDamaged,
                    Reason = d.Reason,
                    Keep = null
                });
            }

            _reportRepo.Add(report);
            return report;
        }

        // 🔹 Duyệt báo cáo
        public void ReviewReport(int reportId, ReviewExportReportDto dto)
        {
            var report = _reportRepo.GetByIdWithDetails(reportId)
                         ?? throw new Exception("Không tìm thấy báo cáo hư hỏng.");

            var export = _exportRepo.GetById(report.ExportId)
                         ?? throw new Exception("Không tìm thấy phiếu xuất.");

            var warehouseId = export.WarehouseId;
            report.DecidedBy = dto.DecidedBy;
            report.DecidedAt = DateTime.UtcNow;

            if (dto.Approve == null)
                throw new Exception("Phải chọn duyệt hoặc từ chối báo cáo.");

            if (dto.Approve == false)
            {
                report.Status = "Rejected";
                _reportRepo.Update(report);

                _handleRequests.Add(new HandleRequest
                {
                    RequestType = "ExportReport",
                    RequestId = reportId,
                    HandledBy = dto.DecidedBy,
                    ActionType = "Rejected",
                    Note = dto.Notes,
                    HandledAt = DateTime.Now
                });
                return;
            }

            if (dto.Details == null || !dto.Details.Any())
                throw new Exception("Thiếu danh sách chi tiết duyệt vật tư.");

            foreach (var d in report.ExportReportDetails)
            {
                var decision = dto.Details.FirstOrDefault(x => x.MaterialId == d.MaterialId)
                               ?? throw new Exception($"Thiếu quyết định cho vật tư ID {d.MaterialId}.");

                d.Keep = decision.Keep;

                if (!d.Keep.Value)
                {
                    var inventory = _inventoryRepo.GetByMaterialId(d.MaterialId, warehouseId)
                        ?? throw new Exception($"Không tìm thấy vật tư {d.MaterialId} trong kho {warehouseId}.");

                    if ((inventory.Quantity ?? 0) < d.QuantityDamaged)
                        throw new Exception($"Không đủ vật tư {d.MaterialId} trong kho {warehouseId}.");

                    inventory.Quantity -= d.QuantityDamaged;
                    _inventoryRepo.Update(inventory);
                }
            }

            _handleRequests.Add(new HandleRequest
            {
                RequestType = "ExportReport",
                RequestId = reportId,
                HandledBy = dto.DecidedBy,
                ActionType = "Approved",
                Note = dto.Notes,
                HandledAt = DateTime.Now
            });

            report.Status = "Approved";
            _reportRepo.Update(report);
        }

        // 🔹 Lấy báo cáo theo ID
        public ExportReportResponseDto GetById(int reportId)
        {
            var report = _reportRepo.GetByIdWithDetails(reportId)
                         ?? throw new Exception("Không tìm thấy báo cáo hư hỏng.");

            var reporter = _userRepo.GetById(report.ReportedBy);

            var details = report.ExportReportDetails.Select(d => new ExportReportDetailResponseDto
            {
                MaterialId = d.MaterialId,
                MaterialName = d.Material?.MaterialName ?? "",
                QuantityDamaged = d.QuantityDamaged,
                Reason = d.Reason,
                Keep = d.Keep ?? false
            }).ToList();

            var lastHandle = _handleRequests.GetByRequest("ExportReport", reportId)
                .OrderByDescending(h => h.HandledAt)
                .FirstOrDefault();

            var handleHistory = lastHandle != null
                ? new List<HandleRequestDto>
                {
            new HandleRequestDto
            {
                HandledBy = lastHandle.HandledBy,
                HandledByName = _userRepo.GetById(lastHandle.HandledBy)?.FullName ?? "",
                ActionType = lastHandle.ActionType,
                Note = lastHandle.Note,
                HandledAt = lastHandle.HandledAt
            }
                }
                : new List<HandleRequestDto>();

            return new ExportReportResponseDto
            {
                ExportReportId = report.ExportReportId,
                ExportId = report.ExportId,
                Status = report.Status,
                ReportedBy = report.ReportedBy,
                ReportedByName = reporter?.FullName ?? "",
                ReportDate = report.ReportDate,
                Notes = report.Notes,
                Details = details,
                HandleHistory = handleHistory
            };
        }


        // 🔹 Lấy tất cả báo cáo theo PartnerId
        public List<ExportReportResponseDto> GetAllByPartner(int partnerId)
        {
            var reports = _reportRepo.GetAllWithDetails()
                .OrderByDescending(r => r.ReportDate)
                .ToList();

            // Lọc theo partnerId của người tạo báo cáo
            var filteredReports = reports
                .Where(r =>
                {
                    var reporter = _userRepo.GetById(r.ReportedBy);
                    return reporter != null && reporter.PartnerId == partnerId;
                })
                .ToList();

            // Lấy bản ghi mới nhất cho mỗi ExportId
            var latestReports = filteredReports
                .GroupBy(r => r.ExportId)
                .Select(g => g.First())
                .ToList();

            var result = new List<ExportReportResponseDto>();

            foreach (var report in latestReports)
            {
                var details = report.ExportReportDetails.Select(d => new ExportReportDetailResponseDto
                {
                    MaterialId = d.MaterialId,
                    MaterialName = d.Material?.MaterialName ?? "",
                    QuantityDamaged = d.QuantityDamaged,
                    Reason = d.Reason,
                    Keep = d.Keep ?? false
                }).ToList();

                // Lấy bản ghi xử lý cuối cùng
                var lastHandle = _handleRequests.GetByRequest("ExportReport", report.ExportReportId)
                    .OrderByDescending(h => h.HandledAt)
                    .FirstOrDefault();

                var handleHistory = lastHandle != null
                    ? new List<HandleRequestDto>
                    {
                new HandleRequestDto
                {
                    HandledBy = lastHandle.HandledBy,
                    HandledByName = _userRepo.GetById(lastHandle.HandledBy)?.FullName ?? "",
                    ActionType = lastHandle.ActionType,
                    Note = lastHandle.Note,
                    HandledAt = lastHandle.HandledAt
                }
                    }
                    : new List<HandleRequestDto>();

                result.Add(new ExportReportResponseDto
                {
                    ExportReportId = report.ExportReportId,
                    ExportId = report.ExportId,
                    Status = report.Status,
                    ReportedBy = report.ReportedBy,
                    ReportedByName = _userRepo.GetById(report.ReportedBy)?.FullName ?? "",
                    ReportDate = report.ReportDate,
                    Notes = report.Notes,
                    Details = details,
                    HandleHistory = handleHistory
                });
            }

            return result;
        }

        // 🔹 Đánh dấu báo cáo là "Đã xem"
        public void MarkAsViewed(int reportId)
        {
            var report = _reportRepo.GetById(reportId)
                         ?? throw new Exception("Không tìm thấy báo cáo hư hỏng.");

            if (report.Status == "Pending")
            {
                report.Status = "Viewed";
                _reportRepo.Update(report);
            }
        }


    }
}
