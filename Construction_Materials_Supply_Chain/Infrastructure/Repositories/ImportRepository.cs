using Infrastructure.Persistence;
using Domain.Interface;
using Domain.Models;
using Infrastructure.Repositories.Base;

namespace Infrastructure.Implementations
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
