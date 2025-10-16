using Domain.Interface;
using Domain.Models;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Implementations
{
    public class ExportReportRepository : GenericRepository<ExportReport>, IExportReportRepository
    {
        public ExportReportRepository(ScmVlxdContext context) : base(context) { }

        public ExportReport? GetByIdWithDetails(int id)
        {
            return _context.ExportReports
                .Include(r => r.ExportReportDetails)
                    .ThenInclude(d => d.Material)
                .FirstOrDefault(r => r.ExportReportId == id);
        }
    }
}
