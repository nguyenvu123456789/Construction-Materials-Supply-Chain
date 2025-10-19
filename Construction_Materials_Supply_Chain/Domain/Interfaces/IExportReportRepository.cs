using Domain.Interface.Base;
using Domain.Models;

namespace Domain.Interface
{
    public interface IExportReportRepository : IGenericRepository<ExportReport>
    {
        ExportReport? GetByIdWithDetails(int id);
        List<ExportReport> GetAllPendingWithDetails();
        List<ExportReport> GetAllReviewedWithDetails();

    }
}
