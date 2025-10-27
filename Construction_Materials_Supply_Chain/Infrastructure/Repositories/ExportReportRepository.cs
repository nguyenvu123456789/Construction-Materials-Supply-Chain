using Domain.Interface;
using Domain.Models;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Base;

namespace Infrastructure.Repositories
{
    public class ExportReportRepository : GenericRepository<ExportReport>, IExportReportRepository
    {
        public ExportReportRepository(ScmVlxdContext context) : base(context)
        {
        }

        public ExportReport? GetByIdWithDetails(int id)
        {
            var report = _dbSet.FirstOrDefault(r => r.ExportReportId == id);
            if (report == null) return null;

            // Load chi tiết thủ công
            report.ExportReportDetails = _context.ExportReportDetails
                .Where(d => d.ExportReportId == id)
                .Select(d => new ExportReportDetail
                {
                    ExportReportDetailId = d.ExportReportDetailId,
                    ExportReportId = d.ExportReportId,
                    MaterialId = d.MaterialId,
                    QuantityDamaged = d.QuantityDamaged,
                    Reason = d.Reason,
                    Keep = d.Keep
                }).ToList();

            return report;
        }

        public List<ExportReport> GetAllWithDetails()
        {
            var reports = _dbSet.OrderByDescending(r => r.ReportDate).ToList();

            var details = _context.ExportReportDetails.ToList();
            var handleRequests = _context.HandleRequests
                .Where(h => h.RequestType == "ExportReport")
                .ToList();

            foreach (var report in reports)
            {
                report.ExportReportDetails = details
                    .Where(d => d.ExportReportId == report.ExportReportId)
                    .ToList();

                // Không dùng navigation, chỉ gán danh sách rỗng hoặc null
                report.GetType().GetProperty("HandleRequests")?.SetValue(report, null);
            }

            return reports;
        }





    }
}
