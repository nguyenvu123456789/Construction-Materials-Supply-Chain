namespace Repositories.Interface
{
    public interface IInventoryRepository
    {
        void DecreaseQuantity(int warehouseId, int materialId, decimal quantity);
        void IncreaseQuantity(int warehouseId, int materialId, decimal quantity);
        bool HasEnoughStock(int warehouseId, int materialId, decimal quantity);
    }
}
