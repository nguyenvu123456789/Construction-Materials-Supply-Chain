using Domain.Models;
using Domain.Interface.Base;

namespace Domain.Interface
{
    public interface IImportRepository : IGenericRepository<Invoice>
    {
        Invoice GetPendingInvoiceByCode(string invoiceCode);
        void ImportInvoice(Invoice invoice, int warehouseId, int createdBy);
    }
}
