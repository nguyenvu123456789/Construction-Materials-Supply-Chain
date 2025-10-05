using BusinessObjects;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using Repositories.Interface;
using System.Collections.Generic;
using System.Linq;

namespace Repositories.Repositories
{
    public class ImportReportRepository : IImportReportRepository
    {
        private readonly ScmVlxdContext _context;

        public ImportReportRepository(ScmVlxdContext context)
        {
            _context = context;
        }

        public void CreateImportReport(ImportReport report)
        {
            _context.ImportReports.Add(report);
            _context.SaveChanges();
        }

        public ImportReport? GetImportReportById(int id)
        {
            return _context.ImportReports
                .Include(r => r.Import)
                .Include(r => r.ImportReportDetails)
                    .ThenInclude(d => d.Material)
                .FirstOrDefault(r => r.ImportReportId == id);
        }

        public List<ImportReport> GetImportReports()
        {
            return _context.ImportReports
                .Include(r => r.Import)
                .Include(r => r.ImportReportDetails)
                .ToList();
        }

        public void UpdateImportReport(ImportReport report)
        {
            _context.ImportReports.Update(report);
            _context.SaveChanges();
        }
    }
}
