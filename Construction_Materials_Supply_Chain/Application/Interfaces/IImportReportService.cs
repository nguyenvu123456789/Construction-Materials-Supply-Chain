using Domain.Models;

namespace Application.Interfaces
{
    public interface IImportReportService
    {
        ImportReport CreateReport(CreateImportReportDto dto);
        ImportReport ReviewReport(int reportId, ReviewImportReportDto dto);
        ImportReport? GetById(int reportId);
        List<ImportReport> GetAllPending();
    }
}
