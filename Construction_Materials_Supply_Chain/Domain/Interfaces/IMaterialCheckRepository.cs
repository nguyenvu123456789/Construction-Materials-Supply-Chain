using Domain.Models;
using Domain.Interface.Base;

namespace Domain.Interface
{
    public interface IMaterialCheckRepository : IGenericRepository<MaterialCheck>
    {
        IQueryable<MaterialCheck> GetAllWithDetails();
    }
}
