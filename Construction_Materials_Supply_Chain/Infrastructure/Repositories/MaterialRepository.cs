using Domain.Interface;
using Domain.Models;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Implementations
{
    public class MaterialRepository : GenericRepository<Material>, IMaterialRepository
    {
        public MaterialRepository(ScmVlxdContext context) : base(context) { }

        public Material? GetByName(string name)
        {
            return _dbSet.FirstOrDefault(m => m.MaterialName == name);
        }

        public bool ExistsByName(string name)
        {
            return _dbSet.Any(m => m.MaterialName == name);
        }

        public Material? GetByIdWithInclude(int id)
        {
            return _dbSet
                .Include(m => m.Category)
                .FirstOrDefault(m => m.MaterialId == id);
        }

        public List<Material> GetAllWithInclude()
        {
            return _dbSet
                .Include(m => m.Category)
                .ToList();
        }
        public List<Material> GetByCategory(int categoryId)
        {
            return _dbSet
                .Include(m => m.Category)
                .Where(m => m.CategoryId == categoryId)
                .ToList();
        }
        public List<Material> GetAllWithInventory()
        {
            return _dbSet
                .Include(m => m.Category)
                .Include(m => m.Inventories)
                    .ThenInclude(i => i.Warehouse)
                .ToList();
        }
        public List<Material> GetByWarehouse(int warehouseId)
        {
            return _dbSet
                .Include(m => m.Category)
                .Include(m => m.Inventories)
                    .ThenInclude(i => i.Warehouse)
                .Where(m => m.Inventories.Any(i => i.WarehouseId == warehouseId))
                .ToList();
        }


    }
}
