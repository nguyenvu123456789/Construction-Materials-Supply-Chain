using BusinessObjects;
using DataAccess;
using Repositories.Interface;

namespace Repositories
{
    public class MaterialRepository : IMaterialRepository
    {
        private readonly MaterialDAO _dao;

        public MaterialRepository(MaterialDAO dao)
        {
            _dao = dao;
        }

        public List<Material> GetMaterials() => _dao.GetMaterials();

        public Material? GetMaterialById(int id) => _dao.GetMaterialById(id);

        public void AddMaterial(Material material) => _dao.AddMaterial(material);

        public void UpdateMaterial(Material material) => _dao.UpdateMaterial(material);

        public void DeleteMaterial(int id) => _dao.DeleteMaterial(id);
    }
}
