using Domain.Interface.Base;
using Domain.Models;

namespace Domain.Interfaces
{
    public interface IRegionRepository : IGenericRepository<Region>
    {
        Region? GetByName(string name);
    }
}
