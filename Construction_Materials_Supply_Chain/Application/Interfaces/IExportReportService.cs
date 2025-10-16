using Application.DTOs;
using Domain.Models;

public interface IExportReportService
{
    ExportReport CreateReport(CreateExportReportDto dto);
    void ReviewReport(int reportId, ReviewExportReportDto dto);
    ExportReport? GetById(int reportId);
    List<ExportReport> GetAllPending();
}
