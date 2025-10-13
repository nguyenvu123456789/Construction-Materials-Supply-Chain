using Domain.Interface.Base;
using Domain.Models;

namespace Domain.Interface
{
    public interface IImportRepository : IGenericRepository<Import>
    {
        Import? GetById(int id);
        List<Import> GetAll();
        Import? GetByIdWithDetails(int id);
    }
}