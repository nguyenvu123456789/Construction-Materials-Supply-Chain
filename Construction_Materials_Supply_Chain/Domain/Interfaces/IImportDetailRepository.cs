using Domain.Interface.Base;
using Domain.Models;

namespace Domain.Interface
{
    public interface IImportDetailRepository : IGenericRepository<ImportDetail>
    {
        List<ImportDetail> GetByImportId(int importId);
    }
}
