using BusinessObjects;

namespace Repositories.Interface
{
    public interface IImportRepository
    {
        Invoice? GetPendingInvoiceByCode(string invoiceCode);
        void ImportInvoice(Invoice invoice, int warehouseId, int createdBy);
    }
}
