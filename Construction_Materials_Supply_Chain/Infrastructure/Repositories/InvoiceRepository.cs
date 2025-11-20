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
            return _dbSet
                .AsNoTracking()
                .Include(i => i.InvoiceDetails)
                    .ThenInclude(d => d.Material)
                .FirstOrDefault(i => i.InvoiceCode.Trim().ToUpper() == invoiceCode.Trim().ToUpper());
        }


        public Invoice? GetByIdWithDetails(int id)
        {
            return _dbSet.Include(i => i.InvoiceDetails)
                         .ThenInclude(d => d.Material)
                         .FirstOrDefault(i => i.InvoiceId == id);
        }

        public List<Invoice> GetAllWithDetails()
        {
            return _dbSet
                .Include(i => i.InvoiceDetails)
                    .ThenInclude(d => d.Material)
                .Include(i => i.Partner)
                .Include(i => i.CreatedByNavigation)
                .AsNoTracking()
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

        public Invoice? GetWithDetailsByCode(string code) =>
                        _dbSet.Include(i => i.InvoiceDetails)
                              .FirstOrDefault(x => x.InvoiceCode == code);

        public Invoice GetWithDetails(int id) => _dbSet.Include(i => i.InvoiceDetails).First(x => x.InvoiceId == id);

        public List<Invoice> GetCustomerImportInvoices(int customerPartnerId)
        {
            return _context.Invoices
                .Where(i => i.InvoiceType == "Import" && i.PartnerId == customerPartnerId)
                .OrderByDescending(i => i.IssueDate)
                .ToList();
        }

        public List<Invoice> GetExportInvoicesByOrderIds(List<int> orderIds)
        {
            if (orderIds == null || orderIds.Count == 0)
                return new List<Invoice>();

            return _context.Invoices
                .Where(i => i.InvoiceType == "Export"
                         && orderIds.Contains(i.OrderId))
                .ToList();
        }
    }
}