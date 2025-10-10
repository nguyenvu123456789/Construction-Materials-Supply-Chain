using Domain.Interface.Base;
using Domain.Models;

namespace Domain.Interface
{
    public interface IExportDetailRepository : IGenericRepository<ExportDetail>
    {
        List<ExportDetail> GetByExportId(int exportId);
    }
}
