using BusinessObjects;
using System.Collections.Generic;

namespace Repositories.Interface
{
    public interface IImportReportRepository
    {
        void CreateImportReport(ImportReport report);
        ImportReport? GetImportReportById(int id);
        List<ImportReport> GetImportReports();
        void UpdateImportReport(ImportReport report);
    }
}
