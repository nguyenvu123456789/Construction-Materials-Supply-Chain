using Domain.Interface.Base;
using Domain.Models;

namespace Domain.Interface
{
    public interface IDriverRepository : IGenericRepository<Driver>
    {
        List<Driver> Search(string? q, bool? active, int? top);
    }
}
