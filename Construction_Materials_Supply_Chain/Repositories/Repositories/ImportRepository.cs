using BusinessObjects;
using DataAccess;
using Repositories.Interface;

namespace Repositories.Repositories
{
    public class ImportRepository : IImportRepository
    {
        private readonly ImportDAO _dao;

        public ImportRepository(ImportDAO dao)
        {
            _dao = dao;
        }

        public Invoice? GetPendingInvoiceByCode(string invoiceCode)
        {
            return _dao.GetPendingInvoiceByCode(invoiceCode);
        }

        public void ImportInvoice(Invoice invoice, int warehouseId, int createdBy)
        {
            _dao.ImportInvoice(invoice, warehouseId, createdBy);
        }
    }
}
