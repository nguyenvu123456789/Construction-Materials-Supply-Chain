using Domain.Interface;
using Domain.Models;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Infrastructure.Implementations
{
    public class AnalystRepository : IAnalystRepository
    {
        private readonly ScmVlxdContext _db;
        public AnalystRepository(ScmVlxdContext db) { _db = db; }

        public IQueryable<Inventory> InventoriesWithWarehouseMaterial()
            => _db.Inventories.AsNoTracking().Include(i => i.Warehouse).Include(i => i.Material);

        public IQueryable<ExportDetail> ExportDetailsWithExport()
            => _db.ExportDetails.AsNoTracking().Include(d => d.Export);

        public IQueryable<InvoiceDetail> InvoiceDetailsWithInvoicePartner()
            => _db.InvoiceDetails.AsNoTracking().Include(d => d.Invoice).ThenInclude(i => i.Partner);

        public IQueryable<Invoice> PurchaseInvoicesWithPartner()
            => _db.Invoices.AsNoTracking().Include(i => i.Partner).Where(i => i.InvoiceType == "Purchase");
    }
}
