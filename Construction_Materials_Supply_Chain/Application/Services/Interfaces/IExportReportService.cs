using Application.DTOs;
using Domain.Models;

public interface IExportReportService
{
    ExportReport CreateReport(CreateExportReportDto dto);
    void ReviewReport(int reportId, ReviewExportReportDto dto);
    ExportReportResponseDto GetById(int reportId);
    List<ExportReportResponseDto> GetAllReports(int? partnerId = null, int? createdByUserId = null);
    void MarkAsViewed(int reportId);

}
