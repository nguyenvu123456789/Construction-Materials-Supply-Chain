using Domain.Interface;
using Domain.Models;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Implementations
{
    public class ImportReportRepository : GenericRepository<ImportReport>, IImportReportRepository
    {
        public ImportReportRepository(ScmVlxdContext context) : base(context) { }

        public ImportReport? GetByIdWithDetails(int id)
        {
            return _context.ImportReports
                .Include(r => r.CreatedByNavigation)
                .Include(r => r.ImportReportDetails)
                    .ThenInclude(d => d.Material)
                .Include(r => r.Import)
                    .ThenInclude(i => i.ImportDetails)
                .Include(r => r.Invoice)
                    .ThenInclude(inv => inv.InvoiceDetails)
                .FirstOrDefault(r => r.ImportReportId == id);
        }

        public IEnumerable<ImportReport> GetAllWithDetails()
        {
            return _context.ImportReports
                .AsNoTracking()
                .Include(r => r.CreatedByNavigation)
                .Include(r => r.ImportReportDetails)
                    .ThenInclude(d => d.Material)
                .Include(r => r.Import)
                .Include(r => r.Invoice)
                .ToList();
        }

        public List<ImportReport> GetAllWithIncludes()
        {
            return _context.ImportReports
                .Include(r => r.CreatedByNavigation)
                .Include(r => r.ImportReportDetails)
                    .ThenInclude(d => d.Material)
                .Include(r => r.Import)
                .Include(r => r.Invoice)
                .ToList();
        }
    }
}
