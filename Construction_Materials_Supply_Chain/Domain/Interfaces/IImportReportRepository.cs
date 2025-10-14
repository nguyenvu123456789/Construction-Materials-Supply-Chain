using Domain.Interface.Base;
using Domain.Models;

namespace Domain.Interface
{
    public interface IImportReportRepository : IGenericRepository<ImportReport>
    {
        ImportReport? GetByIdWithDetails(int id);
    }
}
