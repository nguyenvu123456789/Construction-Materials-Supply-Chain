using Domain.Models;

namespace Application.Interfaces
{
    public interface IMaterialCheckService
    {
        List<MaterialCheck> GetAll();
        MaterialCheck? GetById(int id);
        void Create(MaterialCheck check);
        void Update(MaterialCheck check);
        void Delete(int id);
    }
}
