using Domain.Models;

namespace Domain.Interface
{
    public interface IAnalystRepository
    {
        IQueryable<Inventory> InventoriesWithWarehouseMaterial();
        IQueryable<ExportDetail> ExportDetailsWithExport();
        IQueryable<InvoiceDetail> InvoiceDetailsWithInvoicePartner();
        IQueryable<Invoice> PurchaseInvoicesWithPartner();
    }
}
