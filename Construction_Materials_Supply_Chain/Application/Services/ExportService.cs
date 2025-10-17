using Application.DTOs;
using Application.Interfaces;
using Domain.Interface;
using Domain.Models;

namespace Services.Implementations
{
    public class ExportService : IExportService
    {
        private readonly IExportRepository _exports;
        private readonly IExportDetailRepository _exportDetails;
        private readonly IInventoryRepository _inventories;
        private readonly IMaterialRepository _materialRepository;
        private readonly IInvoiceRepository _invoiceRepository;

        public ExportService(
            IExportRepository exports,
            IExportDetailRepository exportDetails,
            IInventoryRepository inventories,
            IMaterialRepository materialRepository,
            IInvoiceRepository invoiceRepository)
        {
            _exports = exports;
            _exportDetails = exportDetails;
            _inventories = inventories;
            _materialRepository = materialRepository;
            _invoiceRepository = invoiceRepository;
        }

        public Export CreatePendingExport(ExportRequestDto dto)
        {
            if (dto.Materials == null || !dto.Materials.Any())
                throw new Exception("At least one material is required.");

            var export = new Export
            {
                ExportCode = "EXP-" + Guid.NewGuid().ToString("N").Substring(0, 8),
                WarehouseId = dto.WarehouseId,
                CreatedBy = dto.CreatedBy,
                Notes = dto.Notes,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            _exports.Add(export);

            foreach (var m in dto.Materials)
            {
                var material = _materialRepository.GetById(m.MaterialId);
                if (material == null)
                    throw new Exception($"MaterialId {m.MaterialId} not found.");

                var detail = new ExportDetail
                {
                    ExportId = export.ExportId,
                    MaterialId = material.MaterialId,
                    MaterialCode = material.MaterialCode ?? "",
                    MaterialName = material.MaterialName,
                    Unit = material.Unit,
                    Quantity = m.Quantity,
                    UnitPrice = m.UnitPrice,
                    LineTotal = m.Quantity * m.UnitPrice
                };
                _exportDetails.Add(detail);
            }

            return export;
        }

        public Export ConfirmExport(string exportCode, string? notes)
        {
            var export = _exports.GetAll()
                .FirstOrDefault(e => e.ExportCode == exportCode && e.Status == "Pending");

            if (export == null)
                throw new Exception("Pending export not found.");

            var details = _exportDetails.GetByExportId(export.ExportId);
            if (details == null || !details.Any())
                throw new Exception("No export details found.");

            foreach (var detail in details)
            {
                var inventory = _inventories.GetByWarehouseAndMaterial(export.WarehouseId, detail.MaterialId);
                if (inventory == null || (inventory.Quantity ?? 0) < detail.Quantity)
                    throw new Exception($"Not enough quantity in warehouse for material {detail.MaterialId}.");

                inventory.Quantity -= detail.Quantity;
                inventory.UpdatedAt = DateTime.UtcNow;
                _inventories.Update(inventory);
            }

            export.Status = "Success";
            export.Notes = notes ?? export.Notes;
            export.ExportDate = DateTime.UtcNow;
            export.UpdatedAt = DateTime.UtcNow;
            _exports.Update(export);

            return export;
        }

        public Export? RejectExport(int id)
        {
            var export = _exports.GetExportById(id);
            if (export == null)
                return null;

            if (export.Status != "Pending")
                throw new Exception("Only pending exports can be rejected.");

            export.Status = "Rejected";
            export.UpdatedAt = DateTime.UtcNow;
            _exports.Update(export);

            return export;
        }

        public Export? GetById(int id)
        {
            return _exports.GetExportById(id);
        }

        public List<Export> GetAll()
        {
            return _exports.GetAll();
        }
        public Export CreateExportFromInvoice(ExportFromInvoiceDto dto)
        {
            var invoice = _invoiceRepository.GetByCode(dto.InvoiceCode);
            if (invoice == null)
                throw new Exception("Invoice not found.");

            if (invoice.InvoiceDetails == null || !invoice.InvoiceDetails.Any())
                throw new Exception("Invoice has no details.");

            // 🔹 Kiểm tra tồn kho
            foreach (var item in invoice.InvoiceDetails)
            {
                var inventory = _inventories.GetByWarehouseAndMaterial(dto.WarehouseId, item.MaterialId);
                if (inventory == null)
                    throw new Exception($"Material {item.Material?.MaterialName ?? item.MaterialId.ToString()} is not found in this warehouse.");

                if ((inventory.Quantity ?? 0) < item.Quantity)
                    throw new Exception($"Not enough quantity in warehouse for material {item.Material?.MaterialName ?? item.MaterialId.ToString()}.\n" +
                                        $"Available: {inventory.Quantity}, Required: {item.Quantity}");
            }

            // 🔹 Lấy số lớn nhất hiện tại để sinh mã mới
            var exportCode = GenerateNextExportCode();

            // 🔹 Tạo phiếu xuất
            var export = new Export
            {
                ExportCode = exportCode,
                WarehouseId = dto.WarehouseId,
                CreatedBy = dto.CreatedBy,
                Notes = dto.Notes ?? $"Export from Invoice {dto.InvoiceCode}",
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };
            _exports.Add(export);

            // 🔹 Tạo chi tiết phiếu xuất
            foreach (var item in invoice.InvoiceDetails)
            {
                var detail = new ExportDetail
                {
                    ExportId = export.ExportId,
                    MaterialId = item.MaterialId,
                    MaterialCode = item.Material?.MaterialCode ?? "",
                    MaterialName = item.Material?.MaterialName ?? "",
                    Unit = item.Material?.Unit,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    LineTotal = item.Quantity * item.UnitPrice
                };
                _exportDetails.Add(detail);
            }

            // 🔹 Cập nhật trạng thái hóa đơn
            if (invoice.Status?.ToUpper() == "APPROVED")
            {
                invoice.Status = "Exporting";
                invoice.UpdatedAt = DateTime.UtcNow;
                _invoiceRepository.Update(invoice);
            }

            return export;
        }


        private string GenerateNextExportCode()
        {
            int nextNumber = 1;

            // Lấy tất cả ExportCode hiện có, parse số
            var existingNumbers = _exports.GetAll()
                .Select(e =>
                {
                    var parts = e.ExportCode.Split('-');
                    if (parts.Length == 2 && int.TryParse(parts[1], out int n))
                        return n;
                    return 0;
                })
                .Where(n => n > 0)
                .OrderBy(n => n)
                .ToList();

            // Tìm số nhỏ nhất chưa có
            while (existingNumbers.Contains(nextNumber))
                nextNumber++;

            return $"EXP-{nextNumber:000}";
        }



    }
}