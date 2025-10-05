using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace DataAccess
{
    public class ExportReportDAO : BaseDAO
    {
        public ExportReportDAO(ScmVlxdContext context) : base(context) { }

        public ExportReport CreateReport(ExportReport report)
        {
            Context.ExportReports.Add(report);
            Context.SaveChanges();
            return report;
        }

        public ExportReport? GetReportById(int id)
        {
            return Context.ExportReports
                .Include(r => r.ExportReportDetails)
                .Include(r => r.Export)
                .ThenInclude(e => e.Warehouse)
                .FirstOrDefault(r => r.ExportReportId == id);
        }

        public void UpdateReport(ExportReport report)
        {
            Context.ExportReports.Update(report);
            Context.SaveChanges();
        }
    }
}
