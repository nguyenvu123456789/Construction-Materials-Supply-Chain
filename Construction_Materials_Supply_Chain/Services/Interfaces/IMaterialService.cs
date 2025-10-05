using BusinessObjects;

namespace Services.Interfaces
{
    public interface IMaterialService
    {
        List<Material> GetAll();
        Material? GetById(int id);
        void Create(Material material);
        void Update(Material material);
        void Delete(int id);
    }
}
