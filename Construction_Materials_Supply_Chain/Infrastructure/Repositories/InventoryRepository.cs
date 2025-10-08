using Domain.Interface;
using Domain.Models;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Base;
using System.Linq;

namespace Infrastructure.Implementations
{
    public class InventoryRepository : GenericRepository<Inventory>, IInventoryRepository
    {
        public InventoryRepository(ScmVlxdContext context) : base(context) { }

        public Inventory? GetByWarehouseAndMaterial(int warehouseId, int materialId)
        {
            return _dbSet.FirstOrDefault(x => x.WarehouseId == warehouseId && x.MaterialId == materialId);
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
    }
}
