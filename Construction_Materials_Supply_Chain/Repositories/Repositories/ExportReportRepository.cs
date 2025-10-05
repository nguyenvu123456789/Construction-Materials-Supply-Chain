using BusinessObjects;
using DataAccess;
using Repositories.Interface;

namespace Repositories
{
    public class ExportReportRepository : IExportReportRepository
    {
        private readonly ExportReportDAO _dao;

        public ExportReportRepository(ScmVlxdContext context)
        {
            _dao = new ExportReportDAO(context);
        }

        public ExportReport CreateReport(ExportReport report) => _dao.CreateReport(report);

        public ExportReport? GetReportById(int id) => _dao.GetReportById(id);

        public void UpdateReport(ExportReport report) => _dao.UpdateReport(report);
    }
}
