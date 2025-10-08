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

        public ExportService(
            IExportRepository exports,
            IExportDetailRepository exportDetails,
            IInventoryRepository inventories,
            IMaterialRepository materialRepository)
        {
            _exports = exports;
            _exportDetails = exportDetails;
            _inventories = inventories;
            _materialRepository = materialRepository;
        }

        // Tạo phiếu export tạm thời
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

        // Thực hiện xuất kho (Success) → giảm số lượng trong kho
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
            export.UpdatedAt = DateTime.UtcNow;
            _exports.Update(export);

            return export;
        }

        public Export? GetById(int id) => _exports.GetExportById(id);
        public List<Export> GetAll() => _exports.GetAll();
    }
}
