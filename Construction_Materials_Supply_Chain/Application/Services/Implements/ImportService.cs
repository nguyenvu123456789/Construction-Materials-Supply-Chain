using Application.Constants.Enums;
using Application.Constants.Messages;
using Application.DTOs;
using Application.Interfaces;
using Domain.Interface;
using Domain.Models;

namespace Services.Implementations
{
    public class ImportService : IImportService
    {
        private readonly IImportRepository _imports;
        private readonly IInvoiceRepository _invoices;
        private readonly IInventoryRepository _inventories;
        private readonly IImportDetailRepository _importDetails;
        private readonly IMaterialRepository _materialRepository;
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly IMaterialPartnerRepository _materialPartners;

        public ImportService(
            IImportRepository imports,
            IInvoiceRepository invoices,
            IInventoryRepository inventories,
            IImportDetailRepository importDetails,
            IMaterialRepository materialRepository,
            IMaterialPartnerRepository materialPartners,
            IOrderDetailRepository orderDetailRepository)
        {
            _imports = imports;
            _invoices = invoices;
            _inventories = inventories;
            _importDetails = importDetails;
            _materialRepository = materialRepository;
            _materialPartners = materialPartners;
            _orderDetailRepository = orderDetailRepository;
        }

        public Import CreateImportFromInvoice(string? importCode, string? invoiceCode, int warehouseId, int createdBy, string? notes)
        {
            if (string.IsNullOrEmpty(invoiceCode) && string.IsNullOrEmpty(importCode))
                throw new Exception(ImportMessages.MSG_MISSING_INVOICE_OR_IMPORT);

                if (!string.IsNullOrEmpty(invoiceCode))
            {
                var invoice = _invoices.GetByCodeNoTracking(invoiceCode);
                if (invoice == null)
                    throw new Exception(ImportMessages.MSG_INVOICE_NOT_FOUND);

                if (invoice.ImportStatus == ImportStatus.Success.ToString())
                    throw new Exception(ImportMessages.MSG_INVOICE_ALREADY_IMPORTED);

                var import = new Import
                {
                    ImportCode = importCode ?? $"IMP-{Guid.NewGuid():N}".Substring(0, 8),
                    ImportDate = DateTime.Now,
                    WarehouseId = warehouseId,
                    CreatedBy = createdBy,
                    Status = ImportStatus.Success.ToString(),
                    Notes = notes,
                    CreatedAt = DateTime.Now
                };
                _imports.Add(import);

                var materialIds = invoice.InvoiceDetails.Select(d => d.MaterialId).ToList();
                var materials = _materialRepository.GetByIds(materialIds)
                                          .ToDictionary(m => m.MaterialId);

                foreach (var detail in invoice.InvoiceDetails)
                {
                    if (!materials.TryGetValue(detail.MaterialId, out var material))
                        throw new Exception($"Material Id {detail.MaterialId} not found.");

                    var importDetail = new ImportDetail
                    {
                        ImportId = import.ImportId,
                        MaterialId = detail.MaterialId,
                        MaterialCode = material.MaterialCode,
                        MaterialName = material.MaterialName,
                        Unit = material.Unit,
                        UnitPrice = detail.UnitPrice,
                        Quantity = detail.Quantity,
                        LineTotal = detail.UnitPrice * detail.Quantity
                    };
                    _importDetails.Add(importDetail);

                    // Cập nhật tồn kho
                    var inventory = _inventories.GetByWarehouseAndMaterial(warehouseId, detail.MaterialId);
                    if (inventory == null)
                    {
                        _inventories.Add(new Inventory
                        {
                            WarehouseId = warehouseId,
                            MaterialId = detail.MaterialId,
                            Quantity = detail.Quantity,
                            CreatedAt = DateTime.Now
                        });
                    }
                    else
                    {
                        inventory.Quantity = (inventory.Quantity ?? 0) + detail.Quantity;
                        inventory.UpdatedAt = DateTime.Now;
                        _inventories.Update(inventory);
                    }

                    var existingRelation = _materialPartners.GetAll()
    .FirstOrDefault(mp => mp.MaterialId == detail.MaterialId &&
                          mp.PartnerId == invoice.PartnerId);

                    if (existingRelation == null)
                    {
                        _materialPartners.Add(new MaterialPartner
                        {
                            MaterialId = detail.MaterialId,
                            BuyerId = invoice.CreatedBy,
                            PartnerId = invoice.PartnerId
                        });
                    }

                    var orderDetail = _orderDetailRepository.GetByOrderAndMaterial(invoice.OrderId, detail.MaterialId);
                    if (orderDetail != null)
                    {
                        orderDetail.Status = OrderDetailStatus.Success.ToString();
                        _orderDetailRepository.Update(orderDetail);
                    }
                }

                // Cập nhật trạng thái hóa đơn
                invoice.ImportStatus = ImportStatus.Success.ToString();
                invoice.UpdatedAt = DateTime.Now;
                _invoices.Update(invoice);

                return import;
            }

            // Tạo import tự do
            var existingImport = _imports.GetAll().FirstOrDefault(i => i.ImportCode == importCode);
            if (existingImport != null)
            {
                existingImport.Notes = notes ?? existingImport.Notes;
                existingImport.UpdatedAt = DateTime.Now;
                _imports.Update(existingImport);
                return existingImport;
            }

            var newImport = new Import
            {
                ImportCode = importCode!,
                WarehouseId = warehouseId,
                CreatedBy = createdBy,
                Status = ImportStatus.Success.ToString(),
                Notes = notes,
                CreatedAt = DateTime.Now
            };
            _imports.Add(newImport);
            return newImport;

}


        public Import ConfirmPendingImport(string importCode, string? notes)
        {
            var import = _imports.GetAll()
                .FirstOrDefault(i => i.ImportCode == importCode && i.Status == ImportStatus.Pending.ToString());

            if (import == null)
                throw new Exception(ImportMessages.MSG_IMPORT_PENDING_NOT_FOUND);

            var details = _importDetails.GetByImportId(import.ImportId);
            if (details == null || !details.Any())
                throw new Exception(ImportMessages.MSG_IMPORT_DETAIL_NOT_FOUND);

            foreach (var detail in details)
            {
                var inventory = _inventories.GetByWarehouseAndMaterial(import.WarehouseId, detail.MaterialId);
                if (inventory == null)
                {
                    _inventories.Add(new Inventory
                    {
                        WarehouseId = import.WarehouseId,
                        MaterialId = detail.MaterialId,
                        Quantity = detail.Quantity,
                        CreatedAt = DateTime.Now
                    });
                }
                else
                {
                    inventory.Quantity = (inventory.Quantity ?? 0) + detail.Quantity;
                    inventory.UpdatedAt = DateTime.Now;
                    _inventories.Update(inventory);
                }
            }

            import.Status = ImportStatus.Success.ToString();
            import.Notes = notes ?? import.Notes;
            import.UpdatedAt = DateTime.Now;
            _imports.Update(import);

            return import;
        }

        public Import? GetById(int id) => _imports.GetById(id);

        public Import? GetByIdWithDetails(int id)
        {
            var import = _imports.GetById(id);
            if (import != null)
            {
                import.ImportDetails = _importDetails.GetByImportId(id);
            }
            return import;
        }

        public List<Import> GetAll()
        {
            var imports = _imports.GetAll();
            foreach (var import in imports)
            {
                import.ImportDetails = _importDetails.GetByImportId(import.ImportId);
            }
            return imports;
        }

        public Import CreatePendingImport(int warehouseId, int createdBy, string? notes, List<PendingImportMaterialDto> materials)
        {
            if (materials == null || !materials.Any())
                throw new Exception(ImportMessages.MSG_REQUIRE_AT_LEAST_ONE_MATERIAL);

            var import = new Import
            {
                ImportCode = GenerateImportCode(),
                WarehouseId = warehouseId,
                CreatedBy = createdBy,
                Status = ImportStatus.Pending.ToString(),
                Notes = notes,
                CreatedAt = DateTime.Now
            };
            _imports.Add(import);

            foreach (var m in materials)
            {
                var material = _materialRepository.GetById(m.MaterialId);
                if (material == null)
                    throw new Exception(string.Format(ImportMessages.MSG_MATERIAL_NOT_FOUND, m.MaterialId));

                var detail = new ImportDetail
                {
                    ImportId = import.ImportId,
                    MaterialId = material.MaterialId,
                    MaterialCode = material.MaterialCode ?? "",
                    MaterialName = material.MaterialName,
                    Unit = material.Unit,
                    UnitPrice = m.UnitPrice,
                    Quantity = m.Quantity,
                    LineTotal = m.UnitPrice * m.Quantity
                };
                _importDetails.Add(detail);
            }

            return import;
        }

        public Import? RejectImport(int id)
        {
            var import = _imports.GetByIdWithDetails(id);
            if (import == null)
                return null;

            if (import.Status != ImportStatus.Pending.ToString())
                throw new Exception(ImportMessages.MSG_ONLY_PENDING_CAN_BE_REJECTED);

            import.Status = ImportStatus.Rejected.ToString();
            import.UpdatedAt = DateTime.Now;
            _imports.Update(import);

            return import;
        }

        public List<Import> GetImports(int? partnerId = null, int? managerId = null)
        {
            // Lấy tất cả import kèm warehouse và manager
            var imports = _imports.GetAllWithWarehouse();

            // Filter theo partnerId nếu có
            if (partnerId.HasValue)
            {
                imports = imports
                    .Where(i => i.Warehouse?.Manager != null &&
                                i.Warehouse.Manager.PartnerId == partnerId.Value)
                    .ToList();
            }

            // Filter theo managerId nếu có
            if (managerId.HasValue)
            {
                imports = imports
                    .Where(i => i.Warehouse?.ManagerId == managerId.Value)
                    .ToList();
            }

            // Load chi tiết import
            foreach (var import in imports)
            {
                import.ImportDetails = _importDetails.GetByImportId(import.ImportId);
            }

            return imports;
        }
        private string GenerateImportCode()
        {
            var lastImport = _imports
                .GetAll()
                .OrderByDescending(i => i.ImportId)
                .FirstOrDefault();

            int nextNumber = 1;

            if (lastImport != null && !string.IsNullOrEmpty(lastImport.ImportCode))
            {
                var parts = lastImport.ImportCode.Split('_');
                if (parts.Length == 2 && int.TryParse(parts[1], out int currentNumber))
                {
                    nextNumber = currentNumber + 1;
                }
            }

            return $"INV_{nextNumber:D3}";
        }

    }
}
