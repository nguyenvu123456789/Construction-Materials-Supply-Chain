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


        public ImportReport CreateReport(CreateImportReportDto dto)
        {
            if (string.IsNullOrEmpty(dto.InvoiceCode))
                throw new Exception("InvoiceCode is required.");

            // 🔹 Lấy invoice theo code
            var invoice = _invoices.GetByCode(dto.InvoiceCode)
                ?? throw new Exception("Invoice not found.");

            // 🔹 Tạo Import mới (Pending)
            var import = new Import
            {
                ImportCode = $"IMP-{DateTime.UtcNow:yyyyMMddHHmmss}",
                WarehouseId = invoice.PartnerId,
                CreatedBy = dto.CreatedBy,
                CreatedAt = DateTime.UtcNow,
                Status = "Pending"
            };
            _imports.Add(import); // SaveChanges được gọi bên trong

            // 🔹 Tạo ImportReport mới
            var report = new ImportReport
            {
                ImportId = import.ImportId,
                InvoiceId = invoice.InvoiceId,
                CreatedBy = dto.CreatedBy,
                Notes = dto.Notes,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };
            _reports.Add(report); // SaveChanges → report.ImportReportId có giá trị

            // 🔹 Tạo chi tiết report từ client DTO
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

                _reportDetails.Add(detail); // SaveChanges bên trong
            }

            // 🔹 Load lại report từ DB với Include đầy đủ để trả về
            var savedReport = _reports.GetByIdWithDetails(report.ImportReportId)
                ?? throw new Exception("Failed to load created report details.");

            return savedReport;
        }


        public ImportReport ReviewReport(int reportId, ReviewImportReportDto dto)
        {
            var report = _reports.GetByIdWithDetails(reportId)
                         ?? throw new Exception("Report not found.");

            report.Status = dto.Status;
            report.ReviewedBy = dto.ReviewedBy;
            report.ReviewedAt = DateTime.UtcNow;
            report.RejectReason = dto.Status == "Rejected" ? dto.RejectReason : null;

            _reports.Update(report);

            // Lưu lịch sử handle
            var handle = new HandleRequest
            {
                RequestType = "ImportReport",
                RequestId = report.ImportReportId,
                HandledBy = dto.ReviewedBy,
                ActionType = dto.Status,
                Note = dto.Status == "Rejected" ? dto.RejectReason : report.Notes,
                HandledAt = DateTime.UtcNow
            };
            _handleRequests.Add(handle);

            if (dto.Status == "Approved")
            {
                var import = report.Import;
                if (import == null)
                {
                    import = new Import
                    {
                        ImportCode = $"IMP-{DateTime.UtcNow:yyyyMMddHHmmss}",
                        WarehouseId = report.Invoice?.PartnerId ?? 0,
                        CreatedBy = dto.ReviewedBy,
                        CreatedAt = DateTime.UtcNow,
                        Status = "Success"
                    };
                    _imports.Add(import);
                    report.ImportId = import.ImportId;
                    _reports.Update(report);
                }

                // Chuyển vật tư tốt vào ImportDetail + cập nhật tồn kho
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
                        inventory = new Inventory
                        {
                            WarehouseId = import.WarehouseId,
                            MaterialId = material.MaterialId,
                            Quantity = detail.GoodQuantity,
                            UpdatedAt = DateTime.UtcNow
                        };
                        _inventories.Add(inventory);
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
                // Nếu reject → hủy trạng thái invoice về reject

                if (report.Invoice != null)
                {
                    report.Invoice.Status = "Rejected";
                    _invoices.Update(report.Invoice);
                }
            }

            return report;
        }




        public ImportReport? GetById(int reportId)
        {
            return _reports.GetByIdWithDetails(reportId);
        }

        public List<ImportReport> GetAllPending() =>
            _reports.GetAll().Where(r => r.Status == "Pending").ToList();
    }
}
