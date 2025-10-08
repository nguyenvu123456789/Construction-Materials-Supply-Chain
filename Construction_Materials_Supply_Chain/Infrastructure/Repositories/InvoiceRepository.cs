using Domain.Interface;
using Domain.Models;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Implementations
{
    public class InvoiceRepository : GenericRepository<Invoice>, IInvoiceRepository
    {
        public InvoiceRepository(ScmVlxdContext context) : base(context) { }

        public Invoice? GetByCode(string invoiceCode)
        {
            return _dbSet.Include(i => i.InvoiceDetails)
                         .ThenInclude(d => d.Material)
                         .FirstOrDefault(i => i.InvoiceCode == invoiceCode);
        }
    }
}
