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
        private readonly IMaterialPartnerRepository _materialPartners;

        public ImportService(
            IImportRepository imports,
            IInvoiceRepository invoices,
            IInventoryRepository inventories,
            IImportDetailRepository importDetails,
            IMaterialRepository materialRepository,
            IMaterialPartnerRepository materialPartners)
        {
            _imports = imports;
            _invoices = invoices;
            _inventories = inventories;
            _importDetails = importDetails;
            _materialRepository = materialRepository;
            _materialPartners = materialPartners;
        }

        public Import CreateImportFromInvoice(string? importCode, string? invoiceCode, int warehouseId, int createdBy, string? notes)
        {
            if (string.IsNullOrEmpty(invoiceCode) && string.IsNullOrEmpty(importCode))
                throw new Exception(ImportMessages.MSG_MISSING_INVOICE_OR_IMPORT);

            if (!string.IsNullOrEmpty(invoiceCode))
            {
                var invoice = _invoices.GetByCode(invoiceCode);
                if (invoice == null)
                    throw new Exception(ImportMessages.MSG_INVOICE_NOT_FOUND);

                if (invoice.ExportStatus == "Success")
                    throw new Exception(ImportMessages.MSG_INVOICE_ALREADY_IMPORTED);

                var import = new Import
                {
                    ImportCode = importCode ?? "IMP-" + Guid.NewGuid().ToString("N").Substring(0, 8),
                    ImportDate = DateTime.UtcNow,
                    WarehouseId = warehouseId,
                    CreatedBy = createdBy,
                    Status = ImportStatus.Pending.ToString(),
                    Notes = notes,
                    CreatedAt = DateTime.UtcNow
                };
                _imports.Add(import);

                foreach (var detail in invoice.InvoiceDetails)
                {
                    var importDetail = new ImportDetail
                    {
                        ImportId = import.ImportId,
                        MaterialId = detail.MaterialId,
                        MaterialCode = detail.Material.MaterialCode ?? "",
                        MaterialName = detail.Material.MaterialName,
                        Unit = detail.Material.Unit,
                        UnitPrice = detail.UnitPrice,
                        Quantity = detail.Quantity,
                        LineTotal = detail.UnitPrice * detail.Quantity
                    };
                    _importDetails.Add(importDetail);

                    var inventory = _inventories.GetByWarehouseAndMaterial(warehouseId, detail.MaterialId);
                    if (inventory == null)
                    {
                        _inventories.Add(new Inventory
                        {
                            WarehouseId = warehouseId,
                            MaterialId = detail.MaterialId,
                            Quantity = detail.Quantity,
                            UnitPrice = detail.UnitPrice,
                            CreatedAt = DateTime.UtcNow
                        });
                    }
                    else
                    {
                        inventory.Quantity = (inventory.Quantity ?? 0) + detail.Quantity;
                        inventory.UpdatedAt = DateTime.UtcNow;
                        _inventories.Update(inventory);
                    }

                    var existingRelation = _materialPartners
                        .GetAll()
                        .FirstOrDefault(mp => mp.MaterialId == detail.MaterialId
                                           && mp.BuyerId == invoice.CreatedBy
                                           && mp.PartnerId == invoice.PartnerId);
                    if (existingRelation == null)
                    {
                        _materialPartners.Add(new MaterialPartner
                        {
                            MaterialId = detail.MaterialId,
                            BuyerId = invoice.CreatedBy,
                            PartnerId = invoice.PartnerId
                        });
                    }
                }

                invoice.ImportStatus = ImportStatus.Pending.ToString();
                invoice.UpdatedAt = DateTime.UtcNow;
                _invoices.Update(invoice);

                return import;
            }
            else
            {
                var existingImport = _imports.GetAll().FirstOrDefault(i => i.ImportCode == importCode);
                if (existingImport != null)
                {
                    existingImport.Notes = notes ?? existingImport.Notes;
                    existingImport.UpdatedAt = DateTime.UtcNow;
                    _imports.Update(existingImport);
                    return existingImport;
                }

                var newImport = new Import
                {
                    ImportCode = importCode!,
                    WarehouseId = warehouseId,
                    CreatedBy = createdBy,
                    Status = ImportStatus.Pending.ToString(),
                    Notes = notes,
                    CreatedAt = DateTime.UtcNow
                };
                _imports.Add(newImport);
                return newImport;
            }
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
                        UnitPrice = detail.UnitPrice,
                        CreatedAt = DateTime.UtcNow
                    });
                }
                else
                {
                    inventory.Quantity = (inventory.Quantity ?? 0) + detail.Quantity;
                    inventory.UpdatedAt = DateTime.UtcNow;
                    _inventories.Update(inventory);
                }
            }

            import.Status = ImportStatus.Success.ToString();
            import.Notes = notes ?? import.Notes;
            import.UpdatedAt = DateTime.UtcNow;
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
                ImportCode = "REQ-" + Guid.NewGuid().ToString("N").Substring(0, 8),
                WarehouseId = warehouseId,
                CreatedBy = createdBy,
                Status = ImportStatus.Pending.ToString(),
                Notes = notes,
                CreatedAt = DateTime.UtcNow
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
            import.UpdatedAt = DateTime.UtcNow;
            _imports.Update(import);

            return import;
        }

        public List<Import> GetByPartnerId(int partnerId)
        {
            var imports = _imports.GetAllWithWarehouse();
            var filtered = imports
                .Where(i => i.Warehouse != null
                         && i.Warehouse.Manager != null
                         && i.Warehouse.Manager.PartnerId == partnerId)
                .ToList();

            foreach (var import in filtered)
            {
                import.ImportDetails = _importDetails.GetByImportId(import.ImportId);
            }

            return filtered;
        }
    }
}
