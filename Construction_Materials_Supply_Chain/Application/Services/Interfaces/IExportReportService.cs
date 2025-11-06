using Application.DTOs;
using Domain.Models;

public interface IExportReportService
{
    ExportReport CreateReport(CreateExportReportDto dto);
    void ReviewReport(int reportId, ReviewExportReportDto dto);
    ExportReportResponseDto GetById(int reportId);
    List<ExportReportResponseDto> GetAllByPartner(int partnerId);
    void MarkAsViewed(int reportId);

}
