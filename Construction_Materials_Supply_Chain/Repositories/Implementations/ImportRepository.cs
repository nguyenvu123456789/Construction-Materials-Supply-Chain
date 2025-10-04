using System.Linq;
using BusinessObjects;
using DataAccess;
using Repositories.Base;
using Repositories.Interface;

namespace Repositories.Implementations
{
    public class ImportRepository : GenericRepository<Invoice>, IImportRepository
    {
        public ImportRepository(ScmVlxdContext context) : base(context) { }

        public Invoice GetPendingInvoiceByCode(string invoiceCode)
        {
            return _dbSet.FirstOrDefault(x => x.InvoiceCode == invoiceCode);
        }

        public void ImportInvoice(Invoice invoice, int warehouseId, int createdBy)
        {
            _dbSet.Add(invoice);
            _context.SaveChanges();
        }
    }
}
