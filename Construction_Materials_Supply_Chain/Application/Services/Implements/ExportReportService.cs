using Application.Constants.Enums;
using Application.Constants.Messages;
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
        public ExportReport CreateReport(CreateExportReportDto dto)
        {
            var export = _exportRepo.GetById(dto.ExportId)
                         ?? throw new Exception(ExportMessages.EXPORT_NOT_FOUND);

            if (dto.Details == null || !dto.Details.Any())
                throw new Exception(ExportMessages.MSG_REQUIRE_AT_LEAST_ONE_MATERIAL);

            var report = new ExportReport
            {
                ExportId = export.ExportId,
                ExportReportCode = GenerateReportCode(),
                ReportedBy = dto.ReportedBy,
                Notes = dto.Notes,
                Status = StatusEnum.Pending.ToStatusString(),
                ReportDate = DateTime.Now
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

        private string GenerateReportCode()
        {
            var lastReport = _reportRepo.GetAll()
                                .OrderByDescending(r => r.ExportReportId)
                                .FirstOrDefault();

            int nextNumber = 1;
            if (lastReport != null)
            {
                var parts = lastReport.ExportReportCode?.Split('-');
                if (parts != null && parts.Length == 2 && int.TryParse(parts[1], out int lastNumber))
                {
                    nextNumber = lastNumber + 1;
                }
            }

            return $"ERP-{nextNumber:000}";
        }


        // 🔹 Duyệt báo cáo
        public void ReviewReport(int reportId, ReviewExportReportDto dto)
        {
            // Lấy báo cáo với chi tiết
            var report = _reportRepo.GetByIdWithDetails(reportId)
                         ?? throw new Exception(ExportMessages.EXPORT_NOT_FOUND);

            // Lấy phiếu xuất liên quan
            var export = _exportRepo.GetById(report.ExportId)
                         ?? throw new Exception(ExportMessages.EXPORT_NOT_FOUND);

            var warehouseId = export.WarehouseId;
            report.DecidedBy = dto.DecidedBy;
            report.DecidedAt = DateTime.Now;

            if (dto.Approve == null)
                throw new Exception(ExportMessages.INVALID_REQUEST);

            if (dto.Approve == false)
            {
                report.Status = StatusEnum.Rejected.ToStatusString();
                _reportRepo.Update(report);

                _handleRequests.Add(new HandleRequest
                {
                    RequestType = StatusEnum.ExportReport.ToStatusString(),
                    RequestId = reportId,
                    HandledBy = dto.DecidedBy,
                    ActionType = StatusEnum.Rejected.ToStatusString(),
                    Note = dto.Notes,
                    HandledAt = DateTime.Now
                });
                return;
            }

            if (dto.Details == null || !dto.Details.Any())
                throw new Exception(ExportMessages.MSG_REQUIRE_AT_LEAST_ONE_MATERIAL);

            foreach (var d in report.ExportReportDetails)
            {
                var decision = dto.Details.FirstOrDefault(x => x.MaterialId == d.MaterialId)
                               ?? throw new Exception(string.Format(ExportMessages.MSG_MATERIAL_NOT_FOUND, d.MaterialId));

                d.Keep = decision.Keep;

                if (!d.Keep.Value)
                {
                    var inventory = _inventoryRepo.GetByMaterialId(d.MaterialId, warehouseId)
                        ?? throw new Exception(string.Format(ExportMessages.MSG_MATERIAL_NOT_FOUND_IN_WAREHOUSE, d.MaterialId, warehouseId));

                    if ((inventory.Quantity ?? 0) < d.QuantityDamaged)
                        throw new Exception(string.Format(ExportMessages.MSG_NOT_ENOUGH_STOCK, d.MaterialId, inventory.Quantity ?? 0, d.QuantityDamaged));

                    inventory.Quantity -= d.QuantityDamaged;
                    _inventoryRepo.Update(inventory);
                }
            }

            _handleRequests.Add(new HandleRequest
            {
                RequestType = StatusEnum.ExportReport.ToStatusString(),
                RequestId = reportId,
                HandledBy = dto.DecidedBy,
                ActionType = StatusEnum.Approved.ToStatusString(),
                Note = dto.Notes,
                HandledAt = DateTime.Now
            });

            report.Status = StatusEnum.Approved.ToStatusString();
            _reportRepo.Update(report);
        }
        public ExportReportResponseDto GetById(int reportId)
        {
            var report = _reportRepo.GetByIdWithDetails(reportId)
                         ?? throw new Exception(ExportMessages.EXPORT_NOT_FOUND);

            var reporter = _userRepo.GetById(report.ReportedBy);

            var details = report.ExportReportDetails.Select(d => new ExportReportDetailResponseDto
            {
                MaterialId = d.MaterialId,
                MaterialName = d.Material?.MaterialName ?? "",
                QuantityDamaged = d.QuantityDamaged,
                Reason = d.Reason,
                Keep = d.Keep ?? false
            }).ToList();

            var lastHandle = _handleRequests.GetByRequest(StatusEnum.ExportReport.ToStatusString(), reportId)
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
                ExportReportCode = report.ExportReportCode,
                Status = report.Status,
                ReportedBy = report.ReportedBy,
                ReportedByName = reporter?.FullName ?? "",
                ReportDate = report.ReportDate,
                Notes = report.Notes,
                Details = details,
                HandleHistory = handleHistory
            };
        }

        public List<ExportReportResponseDto> GetAllReports(int? partnerId = null, int? createdByUserId = null)
        {
            var reports = _reportRepo.GetAllWithDetails()
                .OrderByDescending(r => r.ReportDate)
                .ToList();

            if (partnerId.HasValue)
            {
                reports = reports
                    .Where(r =>
                    {
                        var reporter = _userRepo.GetById(r.ReportedBy);
                        return reporter != null && reporter.PartnerId == partnerId.Value;
                    })
                    .ToList();
            }

            if (createdByUserId.HasValue)
            {
                reports = reports
                    .Where(r => r.ReportedBy == createdByUserId.Value)
                    .ToList();
            }

            // Lấy bản ghi mới nhất cho mỗi ExportId
            var latestReports = reports
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

                var lastHandle = _handleRequests.GetByRequest(StatusEnum.ExportReport.ToStatusString(), report.ExportReportId)
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
                    ExportReportCode = report.ExportReportCode,
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
        public void MarkAsViewed(int reportId)
        {
            var report = _reportRepo.GetById(reportId)
                         ?? throw new Exception(ExportMessages.EXPORT_NOT_FOUND);
            if (report.Status == StatusEnum.Pending.ToStatusString())
            {
                report.Status = StatusEnum.Viewed.ToStatusString();
                _reportRepo.Update(report);
            }
        }

    }
}
