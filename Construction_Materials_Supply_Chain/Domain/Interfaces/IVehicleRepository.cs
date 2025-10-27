using Domain.Interface.Base;
using Domain.Models;

namespace Domain.Interface
{
    public interface IVehicleRepository : IGenericRepository<Vehicle>
    {
        List<Vehicle> Search(string? q, bool? active, int? top, int? partnerId);
        List<Vehicle> GetByIds(IEnumerable<int> ids);
    }
}
