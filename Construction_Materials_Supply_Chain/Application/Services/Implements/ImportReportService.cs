using Application.DTOs;
using Application.Interfaces;
using Domain.Interface;
using Domain.Models;

namespace Services.Implementations
{
    public class ImportReportService : IImportReportService
    {
        private readonly IImportReportRepository _reports;
        private readonly IInvoiceRepository _invoices;
        private readonly IInventoryRepository _inventories;
        private readonly IImportReportDetailRepository _reportDetails;
        private readonly IMaterialRepository _materials;
        private readonly IImportRepository _imports;
        private readonly IImportService _importService;
        private readonly IImportDetailRepository _importDetails;
        private readonly IHandleRequestRepository _handleRequests;

        public ImportReportService(
            IImportReportRepository reports,
            IInvoiceRepository invoices,
            IInventoryRepository inventories,
            IImportReportDetailRepository reportDetails,
            IMaterialRepository materials,
            IImportRepository imports,
            IImportService importService,
            IImportDetailRepository importDetails,
            IHandleRequestRepository handleRequests)
        {
            _reports = reports;
            _invoices = invoices;
            _inventories = inventories;
            _reportDetails = reportDetails;
            _materials = materials;
            _imports = imports;
            _importService = importService;
            _importDetails = importDetails;
            _handleRequests = handleRequests;
        }

        // 🔹 Tạo mới ImportReport
        public ImportReport CreateReport(CreateImportReportDto dto)
        {
            if (string.IsNullOrEmpty(dto.InvoiceCode))
                throw new Exception("InvoiceCode is required.");

            var invoice = _invoices.GetByCode(dto.InvoiceCode)
                ?? throw new Exception("Invoice not found.");

            // Tạo import tạm (Pending)
            var import = new Import
            {
                ImportCode = $"IMP-{DateTime.UtcNow:yyyyMMddHHmmss}",
                WarehouseId = invoice.PartnerId,
                CreatedBy = dto.CreatedBy,
                CreatedAt = DateTime.UtcNow,
                Status = "Pending"
            };
            _imports.Add(import);

            // Tạo report
            var report = new ImportReport
            {
                ImportId = import.ImportId,
                InvoiceId = invoice.InvoiceId,
                CreatedBy = dto.CreatedBy,
                Notes = dto.Notes,
                CreatedAt = DateTime.UtcNow
            };
            _reports.Add(report);

            // Thêm chi tiết report
            foreach (var clientDetail in dto.Details)
            {
                var detail = new ImportReportDetail
                {
                    ImportReportId = report.ImportReportId,
                    MaterialId = clientDetail.MaterialId,
                    TotalQuantity = clientDetail.TotalQuantity,
                    GoodQuantity = clientDetail.GoodQuantity,
                    DamagedQuantity = clientDetail.DamagedQuantity,
                    Comment = clientDetail.Comment
                };
                _reportDetails.Add(detail);
            }

            // Ghi log xử lý (Pending)
            var handle = new HandleRequest
            {
                RequestType = "ImportReport",
                RequestId = report.ImportReportId,
                HandledBy = dto.CreatedBy,
                ActionType = "Pending",
                Note = "Báo cáo nhập kho được tạo.",
                HandledAt = DateTime.UtcNow
            };
            _handleRequests.Add(handle);

            return _reports.GetByIdWithDetails(report.ImportReportId)
                ?? throw new Exception("Failed to load created report.");
        }

        // 🔹 Duyệt hoặc từ chối ImportReport
        public ImportReportResponseDto ReviewReport(int reportId, ReviewImportReportDto dto)
        {
            var report = _reports.GetByIdWithDetails(reportId)
                         ?? throw new Exception("Report not found.");

            // 🔹 Cập nhật trạng thái ImportReport
            report.Status = dto.Status;
            _reports.Update(report);

            // 🔹 Lưu lịch sử xử lý
            var handle = new HandleRequest
            {
                RequestType = "ImportReport",
                RequestId = report.ImportReportId,
                HandledBy = dto.ReviewedBy,
                ActionType = dto.Status,
                Note = dto.Note,
                HandledAt = DateTime.UtcNow
            };
            _handleRequests.Add(handle);

            // 🔹 Nếu được duyệt
            if (dto.Status == "Approved")
            {
                var import = report.Import ?? new Import
                {
                    ImportCode = $"IMP-{DateTime.UtcNow:yyyyMMddHHmmss}",
                    WarehouseId = report.Invoice?.PartnerId ?? 0,
                    CreatedBy = dto.ReviewedBy,
                    CreatedAt = DateTime.UtcNow,
                    Status = "Success"
                };

                if (report.Import == null)
                {
                    _imports.Add(import);
                    report.ImportId = import.ImportId;
                    _reports.Update(report);
                }

                // Cập nhật tồn kho
                foreach (var detail in report.ImportReportDetails.Where(d => d.GoodQuantity > 0))
                {
                    var material = _materials.GetById(detail.MaterialId)
                        ?? throw new Exception($"Material {detail.MaterialId} not found");

                    // Tạo ImportDetail
                    var importDetail = new ImportDetail
                    {
                        ImportId = import.ImportId,
                        MaterialId = material.MaterialId,
                        MaterialCode = material.MaterialCode ?? "",
                        MaterialName = material.MaterialName,
                        Unit = material.Unit,
                        Quantity = detail.GoodQuantity,
                        UnitPrice = 0,
                        LineTotal = 0
                    };
                    _importDetails.Add(importDetail);

                    // Cập nhật tồn kho
                    var inventory = _inventories.GetByWarehouseAndMaterial(import.WarehouseId, material.MaterialId);
                    if (inventory == null)
                    {
                        _inventories.Add(new Inventory
                        {
                            WarehouseId = import.WarehouseId,
                            MaterialId = material.MaterialId,
                            Quantity = detail.GoodQuantity,
                            UpdatedAt = DateTime.UtcNow
                        });
                    }
                    else
                    {
                        inventory.Quantity += detail.GoodQuantity;
                        inventory.UpdatedAt = DateTime.UtcNow;
                        _inventories.Update(inventory);
                    }
                }
            }
            else if (dto.Status == "Rejected")
            {
                if (report.Invoice != null)
                {
                    report.Invoice.ImportStatus = "Rejected";
                    _invoices.Update(report.Invoice);
                }
            }

            // 🔹 Trả về DTO
            return new ImportReportResponseDto
            {
                ImportReportId = report.ImportReportId,
                Notes = report.Notes,
                CreatedAt = report.CreatedAt,
                ReviewedAt = DateTime.UtcNow,
                Status = report.Status,
                Import = report.Import != null
                    ? new SimpleImportDto
                    {
                        ImportId = report.Import.ImportId,
                        ImportCode = report.Import.ImportCode,
                        CreatedAt = report.Import.CreatedAt ?? DateTime.UtcNow,
                        Status = report.Import.Status
                    }
                    : new SimpleImportDto(),
                Invoice = report.Invoice != null
                    ? new SimpleInvoiceDto
                    {
                        InvoiceId = report.Invoice.InvoiceId,
                        InvoiceCode = report.Invoice.InvoiceCode,
                        InvoiceType = report.Invoice.InvoiceType,
                        IssueDate = report.Invoice.IssueDate
                    }
                    : new SimpleInvoiceDto(),
                Details = report.ImportReportDetails.Select(d => new ImportReportDetailDto
                {
                    MaterialId = d.MaterialId,
                    MaterialCode = d.Material?.MaterialCode ?? "",
                    MaterialName = d.Material?.MaterialName ?? "",
                    TotalQuantity = d.TotalQuantity,
                    GoodQuantity = d.GoodQuantity,
                    DamagedQuantity = d.DamagedQuantity,
                    Comment = d.Comment
                }).ToList()
            };
        }

        public ImportReportResponseDto GetByIdResponse(int reportId)
        {
            var report = _reports.GetByIdWithDetails(reportId)
                         ?? throw new Exception("Không tìm thấy báo cáo nhập kho.");

            // Lấy bản ghi HandleRequest mới nhất
            var lastHandle = _handleRequests.GetByRequest("ImportReport", report.ImportReportId)
                                            .OrderByDescending(h => h.HandledAt)
                                            .FirstOrDefault();

            var reviewerName = lastHandle?.HandledByNavigation?.FullName
                               ?? lastHandle?.HandledByNavigation?.UserName
                               ?? "Chưa duyệt";

            // Tạo 1 HandleRequestDto duy nhất
            var handleHistory = lastHandle != null
                ? new List<HandleRequestDto>
                {
            new HandleRequestDto
            {
                HandledBy = lastHandle.HandledBy,
                HandledByName = reviewerName,
                ActionType = lastHandle.ActionType,
                Note = lastHandle.Note,
                HandledAt = lastHandle.HandledAt
            }
                }
                : new List<HandleRequestDto>();

            // Tên người tạo lấy từ CreatedBy
            var createdByName = "Không rõ"; // mặc định
            var creatorHandle = _handleRequests.GetByRequest("ImportReport", report.ImportReportId)
                                               .OrderBy(h => h.HandledAt)
                                               .FirstOrDefault();
            if (creatorHandle != null)
                createdByName = creatorHandle.HandledByNavigation?.FullName
                                ?? creatorHandle.HandledByNavigation?.UserName
                                ?? "Không rõ";

            return new ImportReportResponseDto
            {
                ImportReportId = report.ImportReportId,
                Notes = report.Notes,
                CreatedAt = report.CreatedAt,
                CreatedBy = report.CreatedBy,
                CreatedByName = createdByName,
                Status = report.Status ?? "Pending",
                Details = report.ImportReportDetails.Select(d => new ImportReportDetailDto
                {
                    MaterialId = d.MaterialId,
                    MaterialCode = d.Material?.MaterialCode ?? "",
                    MaterialName = d.Material?.MaterialName ?? "",
                    TotalQuantity = d.TotalQuantity,
                    GoodQuantity = d.GoodQuantity,
                    DamagedQuantity = d.DamagedQuantity,
                    Comment = d.Comment
                }).ToList(),
                HandleHistory = handleHistory
            };
        }


        // 🔹 Lấy tất cả theo Partner
        public List<ImportReportResponseDto> GetAllByPartner(int? partnerId = null, int? createdByUserId = null)
        {
            var reports = _reports.GetAllWithDetails()
                                  .OrderByDescending(r => r.CreatedAt)
                                  .ToList();

            var result = new List<ImportReportResponseDto>();

            foreach (var report in reports)
            {
                var lastHandle = _handleRequests.GetByRequest("ImportReport", report.ImportReportId)
                                                .OrderByDescending(h => h.HandledAt)
                                                .FirstOrDefault();

                // Filter theo partner nếu có
                if (partnerId.HasValue && lastHandle?.HandledByNavigation?.PartnerId != partnerId.Value)
                    continue;

                // Filter theo người tạo nếu có
                if (createdByUserId.HasValue && report.CreatedBy != createdByUserId.Value)
                    continue;

                var handleHistory = lastHandle != null
                    ? new List<HandleRequestDto>
                    {
                new HandleRequestDto
                {
                    HandledBy = lastHandle.HandledBy,
                    HandledByName = lastHandle.HandledByNavigation?.FullName
                                    ?? lastHandle.HandledByNavigation?.UserName
                                    ?? "Không rõ",
                    ActionType = lastHandle.ActionType,
                    Note = lastHandle.Note,
                    HandledAt = lastHandle.HandledAt
                }
                    }
                    : new List<HandleRequestDto>();

                var createdByName = lastHandle?.HandledByNavigation?.FullName
                                    ?? lastHandle?.HandledByNavigation?.UserName
                                    ?? "Không rõ";

                result.Add(new ImportReportResponseDto
                {
                    ImportReportId = report.ImportReportId,
                    CreatedBy = report.CreatedBy,
                    CreatedByName = createdByName,
                    Notes = report.Notes,
                    CreatedAt = report.CreatedAt,
                    Status = report.Status ?? "Pending",
                    Details = report.ImportReportDetails.Select(d => new ImportReportDetailDto
                    {
                        MaterialId = d.MaterialId,
                        MaterialCode = d.Material?.MaterialCode ?? "",
                        MaterialName = d.Material?.MaterialName ?? "",
                        TotalQuantity = d.TotalQuantity,
                        GoodQuantity = d.GoodQuantity,
                        DamagedQuantity = d.DamagedQuantity,
                        Comment = d.Comment
                    }).ToList(),
                    HandleHistory = handleHistory
                });
            }

            return result;
        }



        public void MarkAsViewed(int reportId)
        {
            var report = _reports.GetById(reportId)
                         ?? throw new Exception("Không tìm thấy báo cáo nhập kho.");

            if (report.Status == "Pending")
            {
                report.Status = "Viewed";
                _reports.Update(report);
            }
        }


    }
}
