using Domain.Interface.Base;
using Domain.Models;

public interface IInventoryRepository : IGenericRepository<Inventory>
{
    Inventory? GetByMaterialId(int materialId, int warehouseId);
    Inventory? GetByMaterial(int materialId);
    Inventory? GetByWarehouseAndMaterial(int warehouseId, int materialId);
    List<Inventory> GetAllByMaterialId(int materialId);
    List<Inventory> GetAllByPartnerId(int partnerId);
}
