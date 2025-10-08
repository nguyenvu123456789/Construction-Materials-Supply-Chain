using Domain.Interface.Base;
using Domain.Models;
using System.Collections.Generic;

namespace Domain.Interface
{
    public interface IExportDetailRepository : IGenericRepository<ExportDetail>
    {
        List<ExportDetail> GetByExportId(int exportId);
    }
}
