using Application.Constants.Enums;
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

        public List<Invoice>? GetInvoiceSeller(int partnerId)
        {
            return _dbSet
                .Where(i => i.CreatedBy == partnerId && i.ExportStatus == "Success" && !_context.Receipts.Any(r => r.Invoices == i.InvoiceCode))
                .ToList();
        }

        public List<Invoice>? GetInvoiceBuyer(int partnerId)
        {
            return _dbSet
                .Where(i => i.PartnerId == partnerId && i.ImportStatus == "Success" || i.ImportStatus == "Delivered" && !_context.Payments.Any(p => p.Invoices == i.InvoiceCode))
                .ToList();
        }

        public Invoice? GetByCode(string invoiceCode)
        {
            return _dbSet
                .Include(i => i.InvoiceDetails)
                    .ThenInclude(d => d.Material)
                    .Include(i => i.Order)
            .ThenInclude(o => o.Warehouse)
                .FirstOrDefault(i => i.InvoiceCode.Trim().ToUpper() == invoiceCode.Trim().ToUpper());
        }
        public Invoice? GetByCodeNoTracking(string invoiceCode)
        {
            return _context.Invoices
                .AsNoTracking()
                .Include(i => i.InvoiceDetails)
                .FirstOrDefault(i => i.InvoiceCode.Trim().ToUpper() == invoiceCode.Trim().ToUpper());
        }

        public Invoice? GetByIdWithDetails(int id)
        {
            return _dbSet
                .Include(i => i.InvoiceDetails)
                         .ThenInclude(d => d.Material)
                         .Include(i => i.Order)
            .ThenInclude(o => o.Warehouse)
                         .FirstOrDefault(i => i.InvoiceId == id);
        }

        public List<Invoice> GetAllWithDetails()
        {
            return _dbSet
                .Include(i => i.Warehouse)
                .Include(i => i.CreatedByNavigation)
                .Include(i => i.Order)
            .ThenInclude(o => o.Warehouse)
                .Include(i => i.InvoiceDetails)
                    .ThenInclude(d => d.Material)
                .Include(i => i.Partner)
                    .ThenInclude(p => p.PartnerRegions)
                        .ThenInclude(pr => pr.Region)
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
                .AsNoTracking()
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

        public List<Invoice> GetCustomerExportInvoices(int customerPartnerId)
        {
            return _context.Invoices
                .Where(i => i.InvoiceType == "Export" && i.PartnerId == customerPartnerId)
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

        public Invoice? GetByIdNoTracking(int id)
        {
            return _dbSet
                .AsNoTracking()
                .Include(i => i.InvoiceDetails)
                    .ThenInclude(d => d.Material)
                .FirstOrDefault(i => i.InvoiceId == id);
        }

        public bool Exists(string invoiceCode)
        {
            return _dbSet.Any(i =>
                i.InvoiceCode.Trim().ToUpper() == invoiceCode.Trim().ToUpper());
        }
        public List<Invoice> GetPendingInvoicesBySellerPartner(int sellerPartnerId)
        {
            return _context.Invoices
                .Include(i => i.CreatedByNavigation)
                    .ThenInclude(u => u.Partner)
                .Include(i => i.Warehouse)
                .Include(i => i.Partner)
                .Where(i =>
                    i.CreatedByNavigation.PartnerId == sellerPartnerId
                    && (
                      i.ImportStatus == StatusEnum.Pending.ToStatusString()
                    )
                )
                .OrderByDescending(i => i.CreatedAt)
                .AsNoTracking()
                .ToList();
        }



    }
}