using Application.DTOs;
using Application.Interfaces;

namespace Application.Services.Implements
{
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryRepository _inventoryRepository;

        public InventoryService(IInventoryRepository inventoryRepository)
        {
            _inventoryRepository = inventoryRepository;
        }

        public List<InventoryInfoDto> GetInventoryFiltered(
            int? partnerId,
            int? managerId,
            string? searchTerm,
            int pageNumber,
            int pageSize,
            out int totalCount)
        {
            // Lấy toàn bộ inventory có includes  
            var inventories = _inventoryRepository.GetAllWithIncludes().AsQueryable();

            // Filter theo partner (nếu có)  
            if (partnerId.HasValue)
            {
                inventories = inventories.Where(i =>
                    i.Warehouse.Manager != null &&
                    i.Warehouse.Manager.PartnerId == partnerId.Value
                );
            }

            // Filter theo manager (nếu có)  
            if (managerId.HasValue)
            {
                inventories = inventories.Where(i =>
                    i.Warehouse.ManagerId == managerId.Value
                );
            }

            // Search theo term  
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                inventories = inventories.Where(i =>
                    (i.Material.MaterialName ?? "").Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    (i.Material.MaterialCode ?? "").Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    (i.Warehouse.WarehouseName ?? "").Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                );
            }

            // Tính tổng count  
            totalCount = inventories.Count();

            // Paging  
            if (pageNumber > 0 && pageSize > 0)
                inventories = inventories.Skip((pageNumber - 1) * pageSize).Take(pageSize);

            // Map sang DTO  
            return inventories
                .ToList()
                .Select(i => new InventoryInfoDto
                {
                    MaterialId = i.MaterialId,
                    MaterialCode = i.Material.MaterialCode,
                    MaterialName = i.Material.MaterialName,
                    CategoryName = i.Material.Category?.CategoryName ?? "N/A",
                    Unit = i.Material.Unit,
                    WarehouseId = i.WarehouseId,
                    WarehouseName = i.Warehouse.WarehouseName,
                    Quantity = i.Quantity ?? 0,
                    BatchNumber = i.BatchNumber,
                    ExpiryDate = i.ExpiryDate
                })
                .ToList();
        }
    }

}
