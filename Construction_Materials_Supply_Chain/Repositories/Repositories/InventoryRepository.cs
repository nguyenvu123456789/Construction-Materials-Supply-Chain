using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using Repositories.Interface;
using System.Linq;

namespace Repositories
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly ScmVlxdContext _context;

        public InventoryRepository(ScmVlxdContext context)
        {
            _context = context;
        }

        public bool HasEnoughStock(int warehouseId, int materialId, decimal quantity)
        {
            var inventory = _context.Inventories
                .FirstOrDefault(i => i.WarehouseId == warehouseId && i.MaterialId == materialId);

            return inventory != null && inventory.Quantity >= quantity;
        }

        public void DecreaseQuantity(int warehouseId, int materialId, decimal quantity)
        {
            var inventory = _context.Inventories
                .FirstOrDefault(i => i.WarehouseId == warehouseId && i.MaterialId == materialId);

            if (inventory == null)
                throw new Exception($"Không tìm thấy vật tư {materialId} trong kho {warehouseId}.");

            if (inventory.Quantity < quantity)
                throw new Exception($"Không đủ hàng. Tồn kho: {inventory.Quantity}, cần: {quantity}.");

            inventory.Quantity -= (int)quantity;
            inventory.UpdatedAt = DateTime.Now;

            _context.SaveChanges();
        }

        public void IncreaseQuantity(int warehouseId, int materialId, decimal quantity)
        {
            var inventory = _context.Inventories
                .FirstOrDefault(i => i.WarehouseId == warehouseId && i.MaterialId == materialId);

            if (inventory == null)
            {
                inventory = new Inventory
                {
                    WarehouseId = warehouseId,
                    MaterialId = materialId,
                    Quantity = (int)quantity,
                    CreatedAt = DateTime.Now
                };
                _context.Inventories.Add(inventory);
            }
            else
            {
                inventory.Quantity += (int)quantity;
                inventory.UpdatedAt = DateTime.Now;
            }

            _context.SaveChanges();
        }
    }
}
