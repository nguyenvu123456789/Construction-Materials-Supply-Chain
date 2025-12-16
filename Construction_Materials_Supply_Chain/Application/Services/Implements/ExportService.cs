using Application.Constants.Enums;
using Application.Constants.Messages;
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

        // Tạo phiếu xuất Pending, kiểm tra tồn kho
        public Export CreatePendingExport(ExportRequestDto dto)
        {
            if (dto.Materials == null || !dto.Materials.Any())
                throw new Exception(ExportMessages.MSG_REQUIRE_AT_LEAST_ONE_MATERIAL);

            foreach (var m in dto.Materials)
            {
                var inventory = _inventories.GetByWarehouseAndMaterial(dto.WarehouseId, m.MaterialId);

                if (inventory == null)
                    throw new Exception(
                        string.Format(ExportMessages.MSG_MATERIAL_NOT_FOUND_IN_WAREHOUSE, m.MaterialId, dto.WarehouseId));

                if ((inventory.Quantity ?? 0) < m.Quantity)
                    throw new Exception(
                        string.Format(ExportMessages.MSG_NOT_ENOUGH_STOCK, m.MaterialId, inventory.Quantity, m.Quantity));
            }

            var export = new Export
            {
                ExportCode = "EXP-" + Guid.NewGuid().ToString("N").Substring(0, 8),
                WarehouseId = dto.WarehouseId,
                CreatedBy = dto.CreatedBy,
                Notes = dto.Notes,
                Status = StatusEnum.Pending.ToStatusString(),   
                CreatedAt = DateTime.Now
            };

            _exports.Add(export);

            foreach (var m in dto.Materials)
            {
                var material = _materialRepository.GetById(m.MaterialId);
                if (material == null)
                    throw new Exception(
                        string.Format(ExportMessages.MSG_MATERIAL_NOT_FOUND, m.MaterialId));

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


        // Xác nhận phiếu xuất Pending
        public Export ConfirmExport(string exportCode, string? notes)
        {
            var export = _exports.GetAll()
                .FirstOrDefault(e => e.ExportCode == exportCode
                                  && e.Status == StatusEnum.Pending.ToStatusString());

            if (export == null)
                throw new Exception(ExportMessages.MSG_PENDING_EXPORT_NOT_FOUND);

            var details = _exportDetails.GetByExportId(export.ExportId);
            if (details == null || !details.Any())
                throw new Exception(ExportMessages.MSG_EXPORT_DETAIL_NOT_FOUND);

            foreach (var detail in details)
            {
                var inventory = _inventories.GetByWarehouseAndMaterial(export.WarehouseId, detail.MaterialId);

                if (inventory == null || (inventory.Quantity ?? 0) < detail.Quantity)
                    throw new Exception(
                        string.Format(ExportMessages.MSG_NOT_ENOUGH_STOCK_WHEN_CONFIRM, detail.MaterialId));

                inventory.Quantity -= detail.Quantity;
                inventory.UpdatedAt = DateTime.Now;
                _inventories.Update(inventory);
            }

            export.Status = StatusEnum.Success.ToStatusString();
            export.Notes = notes ?? export.Notes;
            export.ExportDate = DateTime.Now;
            export.UpdatedAt = DateTime.Now;

            _exports.Update(export);
            return export;
        }


        public Export? RejectExport(int id)
        {
            var export = _exports.GetExportById(id);
            if (export == null)
                return null;
            if (export.Status != StatusEnum.Pending.ToStatusString())
                throw new Exception(ExportMessages.MSG_ONLY_PENDING_CAN_BE_REJECTED);
            export.Status = StatusEnum.Rejected.ToStatusString();
            export.UpdatedAt = DateTime.Now;

            _exports.Update(export);
            return export;
        }

        // Lấy phiếu xuất theo Id
        public Export? GetById(int id)
        {
            return _exports.GetExportById(id);
        }

        // Lấy tất cả phiếu xuất
        public List<Export> GetAll()
        {
            return _exports.GetAll();
        }

        // Tạo phiếu xuất từ hóa đơn (Invoice)

        public Export CreateExportFromInvoice(ExportFromInvoiceDto dto)
            {
        var invoice = _invoiceRepository.GetByCode(dto.InvoiceCode);
        if (invoice == null)
            throw new Exception(ExportMessages.MSG_INVOICE_NOT_FOUND);

        if (invoice.InvoiceDetails == null || !invoice.InvoiceDetails.Any())
            throw new Exception(ExportMessages.MSG_INVOICE_HAS_NO_DETAILS);

        // Kiểm tra tồn kho
        foreach (var item in invoice.InvoiceDetails)
        {
            var inventory = _inventories.GetByWarehouseAndMaterial(dto.WarehouseId, item.MaterialId);

            if (inventory == null)
                throw new Exception(
                    string.Format(ExportMessages.MSG_MATERIAL_NOT_IN_WAREHOUSE,
                        item.Material?.MaterialName ?? item.MaterialId.ToString()));

            if ((inventory.Quantity ?? 0) < item.Quantity)
                throw new Exception(
                    string.Format(ExportMessages.MSG_NOT_ENOUGH_STOCK,
                        item.Material?.MaterialName ?? item.MaterialId.ToString(),
                        inventory.Quantity,
                        item.Quantity));
        }

        var exportCode = GenerateNextExportCode();

        var export = new Export
        {
            ExportCode = exportCode,
            WarehouseId = dto.WarehouseId,
            CreatedBy = dto.CreatedBy,
            Notes = dto.Notes ?? $"Export from Invoice {dto.InvoiceCode}",
            Status = StatusEnum.Pending.ToStatusString(),
            CreatedAt = DateTime.Now,
        };
        _exports.Add(export);

        // Tạo chi tiết phiếu xuất
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

        invoice.ExportStatus = StatusEnum.Success.ToStatusString();
        invoice.UpdatedAt = DateTime.Now;
        _invoiceRepository.Update(invoice);

        return export;
            }


    // Sinh mã phiếu xuất tăng dần
        private string GenerateNextExportCode()
        {
            int nextNumber = 1;

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

            while (existingNumbers.Contains(nextNumber))
                nextNumber++;

            return $"EXP-{nextNumber:000}";
        }

        // Lấy phiếu xuất theo Partner
        public List<Export> GetByPartnerOrManager(int? partnerId = null, int? managerId = null)
        {
            var exports = _exports.GetAllWithWarehouse().AsQueryable();

            if (partnerId.HasValue)
            {
                exports = exports.Where(e =>
                    e.Warehouse != null &&
                    e.Warehouse.Manager != null &&
                    e.Warehouse.Manager.PartnerId == partnerId.Value
                );
            }

            if (managerId.HasValue)
            {
                exports = exports.Where(e =>
                    e.Warehouse != null &&
                    e.Warehouse.ManagerId == managerId.Value
                );
            }

            var filtered = exports.ToList();

            foreach (var export in filtered)
            {
                export.ExportDetails = _exportDetails.GetByExportId(export.ExportId);
            }

            return filtered;
        }
    }
}
