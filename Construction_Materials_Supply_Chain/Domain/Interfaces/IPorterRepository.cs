using Domain.Interface.Base;
using Domain.Models;
using System.Linq.Expressions;

namespace Domain.Interface
{
    public interface IPorterRepository : IGenericRepository<Porter>
    {
        List<Porter> Search(string? q, bool? active, int? top, int? partnerId);
        List<Porter> GetByIds(IEnumerable<int> ids);
        bool CheckExists(Expression<Func<Porter, bool>> predicate);
    }
}
