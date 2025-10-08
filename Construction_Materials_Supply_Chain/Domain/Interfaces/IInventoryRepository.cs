using Domain.Interface.Base;
using Domain.Models;

namespace Domain.Interface
{
    public interface IInventoryRepository : IGenericRepository<Inventory>
    {
        Inventory? GetByWarehouseAndMaterial(int warehouseId, int materialId);
    }
}
