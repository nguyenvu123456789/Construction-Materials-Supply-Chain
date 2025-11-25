using Domain.Interface.Base;
using Domain.Models;

namespace Domain.Interface
{
    public interface IMaterialRepository : IGenericRepository<Material>
    {
        Material? GetByName(string name);
        bool ExistsByName(string name);
        Material? GetByIdWithInclude(int id);
        List<Material> GetAllWithInclude();
        List<Material> GetByCategory(int categoryId);
        List<Material> GetByWarehouse(int warehouseId);
        List<Material> GetAllWithInventory();
        Material? GetDetailById(int id);
        void AddInventory(Inventory inventory);
        List<Material> GetByIds(List<int> materialIds);
    }
}
