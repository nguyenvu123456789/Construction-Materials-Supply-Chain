using Domain.Interface;
using Domain.Models;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Repositories
{
    public class ExportReportRepository : GenericRepository<ExportReport>, IExportReportRepository
    {
        public ExportReportRepository(ScmVlxdContext context) : base(context)
        {
        }

        public ExportReport? GetByIdWithDetails(int id)
        {
            return _dbSet
                .Include(r => r.ExportReportDetails)
                    .ThenInclude(d => d.Material) 
                .FirstOrDefault(r => r.ExportReportId == id);
        }

        public List<ExportReport> GetAllPendingWithDetails()
        {
            return _dbSet
                .Include(r => r.ExportReportDetails)
                    .ThenInclude(d => d.Material) 
                .Where(r => r.Status == "Pending")
                .ToList();
        }
        public List<ExportReport> GetAllReviewedWithDetails()
        {
            return _context.ExportReports
                .Include(r => r.ExportReportDetails)
                .Where(r => r.Status != "Pending")
                .OrderByDescending(r => r.ReportDate)
                .ToList();
        }

    }
}
