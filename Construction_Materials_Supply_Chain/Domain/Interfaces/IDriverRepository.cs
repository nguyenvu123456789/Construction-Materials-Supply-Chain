using Domain.Interface.Base;
using Domain.Models;
using System.Linq.Expressions;

namespace Domain.Interface
{
    public interface IDriverRepository : IGenericRepository<Driver>
    {
        List<Driver> Search(string? q, bool? active, int? top, int? partnerId);
        List<Driver> GetByIds(IEnumerable<int> ids);
        bool CheckExists(Expression<Func<Driver, bool>> predicate);
    }
}
