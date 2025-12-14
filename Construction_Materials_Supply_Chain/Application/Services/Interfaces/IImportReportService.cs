using Domain.Models;

namespace Application.Interfaces
{
    public interface IImportReportService
    {
        ImportReport CreateReport(CreateImportReportDto dto);
        List<ImportReportResponseDto> GetAllByPartner(int? partnerId = null, int? createdByUserId = null);
        ImportReportResponseDto GetByIdResponse(int id);
        ImportReportResponseDto ReviewReport(int id, ReviewImportReportDto dto);
        void MarkAsViewed(int reportId);
        Import ReviewReturnImport(int importId, ReviewImportReportDto dto);

    }
}
