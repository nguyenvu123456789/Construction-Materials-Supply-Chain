using Domain.Models;

namespace Domain.Interface
{
    public interface IMaterialCheckDetailRepository
    {
        IQueryable<MaterialCheckDetail> GetAll();
        Task AddAsync(MaterialCheckDetail entity);
        Task<int> SaveChangesAsync();
    }
}
