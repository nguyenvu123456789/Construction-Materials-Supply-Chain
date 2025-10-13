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

        public Invoice? GetByIdWithDetails(int id)
        {
            return _dbSet.Include(i => i.InvoiceDetails)
                         .ThenInclude(d => d.Material)
                         .FirstOrDefault(i => i.InvoiceId == id);
        }

        public List<Invoice> GetAllWithDetails()
        {
            return _dbSet.Include(i => i.InvoiceDetails)
                         .ThenInclude(d => d.Material)
                         .ToList();
        }

        // Phương thức Add kế thừa từ GenericRepository, không cần triển khai lại
    }
}