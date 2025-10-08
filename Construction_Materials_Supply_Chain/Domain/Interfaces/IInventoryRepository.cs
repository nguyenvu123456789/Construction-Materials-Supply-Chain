using Domain.Interface.Base;
using Domain.Models;

public interface IInventoryRepository : IGenericRepository<Inventory>
{
    Inventory? GetByMaterial(int materialId);
    Inventory? GetByWarehouseAndMaterial(int warehouseId, int materialId);
}
