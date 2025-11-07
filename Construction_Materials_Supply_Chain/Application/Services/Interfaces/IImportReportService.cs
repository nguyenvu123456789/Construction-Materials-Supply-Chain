using Domain.Models;

namespace Application.Interfaces
{
    public interface IImportReportService
    {
        ImportReport CreateReport(CreateImportReportDto dto);
        List<ImportReportResponseDto> GetAll();
        ImportReportResponseDto GetByIdResponse(int id);
        ImportReportResponseDto ReviewReport(int id, ReviewImportReportDto dto);
        void MarkAsViewed(int reportId);

    }
}
