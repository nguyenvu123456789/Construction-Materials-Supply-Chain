using Domain.Interface;
using Domain.Models;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Implementations
{
    public class ExportRepository : GenericRepository<Export>, IExportRepository
    {
        public ExportRepository(ScmVlxdContext context) : base(context) { }

        public Export GetExportById(int id)
        {
            return _dbSet
                .Include(e => e.Invoice)
                .Include(e => e.ExportDetails)
                    .ThenInclude(d => d.Material)
                .FirstOrDefault(e => e.ExportId == id);
        }

        public override List<Export> GetAll()
        {
            return _dbSet.Include(e => e.ExportDetails).ToList();
        }

        public Export GetWithDetails(int id) => _dbSet.Include(e => e.ExportDetails).First(x => x.ExportId == id);
        public List<Export> GetAllWithWarehouse()
        {
            return _context.Exports
                .Include(e => e.Invoice)
                .Include(e => e.Warehouse)
                    .ThenInclude(w => w.Manager)
                .Include(e => e.ExportDetails)
                .ToList();
        }
        public Export? GetByInvoiceId(int invoiceId)
        {
            return _context.Exports
                .Include(e => e.ExportDetails)
                .Include(e => e.Invoice)
                    .FirstOrDefault(e => e.InvoiceId == invoiceId);
        }


    }
}