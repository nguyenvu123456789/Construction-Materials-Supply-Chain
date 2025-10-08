using Domain.Interface.Base;
using Domain.Models;
using System.Collections.Generic;

namespace Domain.Interface
{
    public interface IImportDetailRepository : IGenericRepository<ImportDetail>
    {
        List<ImportDetail> GetByImportId(int importId);
    }
}
