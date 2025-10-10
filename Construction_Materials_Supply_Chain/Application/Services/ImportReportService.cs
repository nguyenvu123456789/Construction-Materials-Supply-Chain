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


        public ImportReportService(
            IImportReportRepository reports,
            IInvoiceRepository invoices,
            IInventoryRepository inventories,
            IImportReportDetailRepository reportDetails,
            IMaterialRepository materials,
            IImportRepository imports,
            IImportService importService) 
        {
            _reports = reports;
            _invoices = invoices;
            _inventories = inventories;
            _reportDetails = reportDetails;
            _materials = materials;
            _imports = imports;
            _importService = importService; // lưu instance
        }


        // 🔹 Tạo phiếu báo cáo theo InvoiceId
        public ImportReport CreateReport(CreateImportReportDto dto)
        {
            if (string.IsNullOrEmpty(dto.InvoiceCode))
                throw new Exception("InvoiceCode is required.");

            var invoice = _invoices.GetByCode(dto.InvoiceCode)
                ?? throw new Exception("Invoice not found.");

            // 🔹 Tạo Import tự sinh
            var import = new Import
            {
                ImportCode = $"IMP-{DateTime.UtcNow:yyyyMMddHHmmss}",
                WarehouseId = invoice.PartnerId, // hoặc warehouse mặc định
                CreatedBy = dto.CreatedBy,
                CreatedAt = DateTime.UtcNow
            };
            _imports.Add(import);

            var report = new ImportReport
            {
                ImportId = import.ImportId,
                InvoiceId = invoice.InvoiceId,
                CreatedBy = dto.CreatedBy,
                Notes = dto.Notes,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };
            _reports.Add(report);

            // 🔹 Tạo chi tiết report từ Invoice + dữ liệu client
            foreach (var invoiceDetail in invoice.InvoiceDetails)
            {
                // Tìm chi tiết do client gửi theo MaterialId
                var clientDetail = dto.Details.FirstOrDefault(d => d.MaterialId == invoiceDetail.MaterialId);
                if (clientDetail == null)
                    throw new Exception($"Missing detail for MaterialId {invoiceDetail.MaterialId}");

                var detail = new ImportReportDetail
                {
                    ImportReportId = report.ImportReportId,
                    MaterialId = invoiceDetail.MaterialId,
                    TotalQuantity = invoiceDetail.Quantity,         
                    GoodQuantity = clientDetail.GoodQuantity,      
                    DamagedQuantity = clientDetail.DamagedQuantity,
                    Comment = clientDetail.Comment
                };

                _reportDetails.Add(detail);
                report.ImportReportDetails.Add(detail);
            }

            return report;
        }





        // 🔹 Duyệt phiếu báo cáo và tự nhập kho sản phẩm tốt
        public ImportReport ReviewReport(int reportId, ReviewImportReportDto dto)
        {
            // Lấy report
            var report = _reports.GetById(reportId);
            if (report == null) throw new Exception("Report not found.");

            // Cập nhật trạng thái review
            report.Status = dto.Status;
            report.ReviewedBy = dto.ReviewedBy;
            report.ReviewedAt = DateTime.Now;
            report.RejectReason = dto.Status == "Rejected" ? dto.RejectReason : null;

            _reports.Update(report);

            // Nếu Approved, tạo PendingImport và tự xác nhận nhập kho
            if (dto.Status == "Approved")
            {
                var goodMaterials = report.ImportReportDetails
                    .Where(d => d.GoodQuantity > 0)
                    .Select(d => new Application.DTOs.PendingImportMaterialDto
                    {
                        MaterialId = d.MaterialId,
                        Quantity = d.GoodQuantity
                    })
                    .ToList();

                if (goodMaterials.Any())
                {
                    // 1️⃣ Tạo phiếu Pending Import
                    var pendingImport = _importService.CreatePendingImport(
                        warehouseId: report.Import.WarehouseId,
                        createdBy: dto.ReviewedBy,
                        notes: $"Auto-generated from approved report #{reportId}",
                        materials: goodMaterials
                    );

                    // 2️⃣ Xác nhận Pending Import để thực sự nhập kho
                    _importService.ConfirmPendingImport(
                        importCode: pendingImport.ImportCode,
                        notes: $"Auto-import from approved report #{reportId}"
                    );
                }
            }

            return report;
        }

        public ImportReport? GetById(int reportId) => _reports.GetById(reportId);

        public List<ImportReport> GetAllPending() =>
            _reports.GetAll().Where(r => r.Status == "Pending").ToList();
    }
}
