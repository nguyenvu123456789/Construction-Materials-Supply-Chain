using BusinessObjects;

namespace Repositories.Interface
{
    public interface IExportReportRepository
    {
        ExportReport CreateReport(ExportReport report);
        ExportReport? GetReportById(int id);
        void UpdateReport(ExportReport report);
    }
}
