using Domain.Interface.Base;
using Domain.Models;
using System.Linq.Expressions;

namespace Domain.Interface
{
    public interface IVehicleRepository : IGenericRepository<Vehicle>
    {
        List<Vehicle> Search(string? q, bool? active, int? top, int? partnerId);
        List<Vehicle> GetByIds(IEnumerable<int> ids);
        bool CheckExists(Expression<Func<Vehicle, bool>> predicate);
    }
}
