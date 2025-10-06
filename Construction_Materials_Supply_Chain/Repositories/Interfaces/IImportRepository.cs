using Domain;
using Infrastructure.Base;

namespace Infrastructure.Interface
{
    public interface IImportRepository : IGenericRepository<Invoice>
    {
        Invoice GetPendingInvoiceByCode(string invoiceCode);
        void ImportInvoice(Invoice invoice, int warehouseId, int createdBy);
    }
}
