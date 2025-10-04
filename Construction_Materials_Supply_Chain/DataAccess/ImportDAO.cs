using BusinessObjects;
using Microsoft.EntityFrameworkCore;

namespace DataAccess
{
    public class ImportDAO : BaseDAO
    {
        public ImportDAO(ScmVlxdContext context) : base(context) { }

        public Invoice? GetPendingInvoiceByCode(string invoiceCode)
        {
            return Context.Invoices
                .Include(i => i.InvoiceDetails)
                    .ThenInclude(d => d.Material)
                .FirstOrDefault(i => i.InvoiceCode == invoiceCode && i.Status == "Pending");
        }

        public void ImportInvoice(Invoice invoice, int warehouseId, int createdBy)
        {
            if (invoice == null) throw new ArgumentNullException(nameof(invoice));

            var now = DateTime.Now;

            // 1. Tạo Import
            var import = new Import
            {
                WarehouseId = warehouseId,
                CreatedAt = now,
                CreatedBy = createdBy
            };
            Context.Imports.Add(import);
            Context.SaveChanges(); // cần để lấy ImportId

            // 2. Tạo ImportDetail từ InvoiceDetail
            foreach (var detail in invoice.InvoiceDetails)
            {
                var importDetail = new ImportDetail
                {
                    ImportId = import.ImportId,
                    MaterialCode = detail.Material.MaterialCode,
                    MaterialName = detail.Material.MaterialName,
                    Unit = detail.Material.Unit,
                    UnitPrice = detail.UnitPrice,
                    Quantity = detail.Quantity,
                    LineTotal = detail.LineTotal ?? detail.Quantity * detail.UnitPrice
                };
                Context.ImportDetails.Add(importDetail);

                // 3. Cập nhật Inventory
                var inventory = Context.Inventories
                    .FirstOrDefault(inv => inv.WarehouseId == warehouseId && inv.MaterialId == detail.MaterialId);

                if (inventory != null)
                {
                    inventory.Quantity = (inventory.Quantity ?? 0) + detail.Quantity;
                    inventory.UpdatedAt = now;
                    Context.Entry(inventory).State = EntityState.Modified;
                }
                else
                {
                    Context.Inventories.Add(new Inventory
                    {
                        WarehouseId = warehouseId,
                        MaterialId = detail.MaterialId,
                        Quantity = detail.Quantity,
                        UnitPrice = detail.UnitPrice,
                        CreatedAt = now
                    });
                }
            }

            // 4. Cập nhật trạng thái Invoice
            invoice.Status = "Success";
            invoice.UpdatedAt = now;
            Context.Entry(invoice).State = EntityState.Modified;

            Context.SaveChanges();
        }
    }
}
