using BusinessObjects;

namespace Repositories.Interface
{
    public interface IMaterialRepository
    {
        List<Material> GetMaterials();
        Material? GetMaterialById(int id);
        void AddMaterial(Material material);
        void UpdateMaterial(Material material);
        void DeleteMaterial(int id);
    }
}
