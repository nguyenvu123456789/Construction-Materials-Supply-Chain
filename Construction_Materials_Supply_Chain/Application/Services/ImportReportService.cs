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

            // Lưu lịch sử xử lý
            var handle = new HandleRequest
            {
                RequestType = "ImportReport",
                RequestId = report.ImportReportId,
                HandledBy = dto.ReviewedBy,
                ActionType = dto.Status,
                Note = dto.Status == "Rejected" ? dto.RejectReason : report.Notes, // 🔹 không dùng dto.Notes nữa
                HandledAt = DateTime.UtcNow
            };
            _handleRequests.Add(handle);

            // Nếu được duyệt
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
                    report.Invoice.Status = "Rejected";
                    _invoices.Update(report.Invoice);
                }
            }

            // Tạo response DTO
            return new ImportReportResponseDto
            {
                ImportReportId = report.ImportReportId,
                Notes = report.Notes,
                CreatedAt = report.CreatedAt,
                ReviewedAt = DateTime.UtcNow,
                RejectReason = dto.RejectReason,
                Status = dto.Status,
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


        // 🔹 Lấy theo ID
        public ImportReport? GetById(int reportId)
        {
            return _reports.GetByIdWithDetails(reportId);
        }

        // 🔹 Lấy tất cả báo cáo chưa duyệt (Pending)
        public List<ImportReport> GetAllPending()
        {
            // Lấy toàn bộ ImportReport
            var allReports = _reports.GetAll();

            // Lọc những cái chưa có HandleRequest Approved/Rejected
            var pendingIds = allReports
                .Where(r =>
                    !_handleRequests.Exists("ImportReport", r.ImportReportId, new[] { "Approved", "Rejected" }))
                .Select(r => r.ImportReportId)
                .ToList();

            return allReports.Where(r => pendingIds.Contains(r.ImportReportId)).ToList();
        }
    }
}
