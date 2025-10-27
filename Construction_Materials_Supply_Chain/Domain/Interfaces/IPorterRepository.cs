using Domain.Interface.Base;
using Domain.Models;

namespace Domain.Interface
{
    public interface IPorterRepository : IGenericRepository<Porter>
    {
        List<Porter> Search(string? q, bool? active, int? top, int? partnerId);
        List<Porter> GetByIds(IEnumerable<int> ids);
    }
}
