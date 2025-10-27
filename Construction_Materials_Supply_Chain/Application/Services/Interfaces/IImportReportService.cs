using Domain.Models;

namespace Application.Interfaces
{
    public interface IImportReportService
    {
        ImportReport CreateReport(CreateImportReportDto dto);
        ImportReport? GetById(int id);
        List<ImportReport> GetAllPending();
        ImportReportResponseDto ReviewReport(int id, ReviewImportReportDto dto);
    }
}
