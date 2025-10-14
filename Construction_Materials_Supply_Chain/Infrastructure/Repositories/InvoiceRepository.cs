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

            public List<Invoice> GetAll() => _dbSet.ToList();
            public void AddInvoiceDetail(InvoiceDetail detail)
            {
                _context.Set<InvoiceDetail>().Add(detail);
                _context.SaveChanges();
            }

            public List<InvoiceDetail> GetInvoiceDetailsByInvoiceId(int invoiceId)
            {
                return _context.Set<InvoiceDetail>()
                    .Where(d => d.InvoiceId == invoiceId)
                    .Include(d => d.Material)
                    .ToList();
            }

        }
    }