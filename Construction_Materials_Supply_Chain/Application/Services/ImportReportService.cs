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


        public ImportReportService(
            IImportReportRepository reports,
            IInvoiceRepository invoices,
            IInventoryRepository inventories,
            IImportReportDetailRepository reportDetails,
            IMaterialRepository materials,
            IImportRepository imports)   
        {
            _reports = reports;
            _invoices = invoices;
            _inventories = inventories;
            _reportDetails = reportDetails;
            _materials = materials;
            _imports = imports;  
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





        // 🔹 Duyệt phiếu báo cáo
        public ImportReport ReviewReport(int reportId, ReviewImportReportDto dto)
        {
            var report = _reports.GetById(reportId) ?? throw new Exception("Report not found.");
            if (report.Status != "Pending") throw new Exception("Report already reviewed.");

            report.Status = dto.Status;
            report.ReviewedBy = dto.ReviewedBy;
            report.ReviewedAt = DateTime.UtcNow;
            report.RejectReason = dto.RejectReason;

            if (dto.Status == "Approved")
            {
                // nhập số lượng Good vào kho theo từng vật tư
                foreach (var detail in report.ImportReportDetails)
                {
                    // giả sử Inventory lưu theo MaterialId duy nhất hoặc warehouse mặc định
                    var inventory = _inventories.GetByMaterial(detail.MaterialId);
                    if (inventory == null)
                    {
                        _inventories.Add(new Inventory
                        {
                            MaterialId = detail.MaterialId,
                            Quantity = detail.GoodQuantity,
                            CreatedAt = DateTime.UtcNow
                        });
                    }
                    else
                    {
                        inventory.Quantity = (inventory.Quantity ?? 0) + detail.GoodQuantity;
                        inventory.UpdatedAt = DateTime.UtcNow;
                        _inventories.Update(inventory);
                    }
                }
            }

            _reports.Update(report);
            return report;
        }

        public ImportReport? GetById(int reportId) => _reports.GetById(reportId);

        public List<ImportReport> GetAllPending() =>
            _reports.GetAll().Where(r => r.Status == "Pending").ToList();
    }
}
