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
    }
}
