using Domain.Models;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Implementations
{
    public class InventoryRepository : GenericRepository<Inventory>, IInventoryRepository
    {
        public InventoryRepository(ScmVlxdContext context) : base(context) { }

        public Inventory? GetByWarehouseAndMaterial(int warehouseId, int materialId)
        {
            return _dbSet.AsNoTracking().
                FirstOrDefault(x => x.WarehouseId == warehouseId && x.MaterialId == materialId);
        }
        public Inventory? GetByMaterial(int materialId)
        {
            return _context.Inventories.FirstOrDefault(i => i.MaterialId == materialId);
        }
        public Inventory? GetByMaterialId(int materialId, int warehouseId)
        {
            return _context.Inventories
                           .FirstOrDefault(i => i.MaterialId == materialId && i.WarehouseId == warehouseId);
        }
        public List<Inventory> GetAllByMaterialId(int materialId)
        {
            return _context.Inventories
                           .Include(i => i.Material)
                           .Include(i => i.Warehouse)
                           .Where(i => i.MaterialId == materialId)
                           .ToList();
        }
        public IQueryable<Inventory> GetAllWithIncludes()
        {
            return _context.Inventories
                .Include(i => i.Material)
                    .ThenInclude(m => m.Category)
                .Include(i => i.Warehouse)
                    .ThenInclude(w => w.Manager);
        }

    }
}
