using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Interface;

namespace Application.Services.Implements
{
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IMapper _mapper;

        public InventoryService(IInventoryRepository inventoryRepository, IMapper mapper)
        {
            _inventoryRepository = inventoryRepository;
            _mapper = mapper;
        }

        public List<InventoryInfoDto> GetInventoryByPartnerFiltered(
            int partnerId,
            string? searchTerm,
            int pageNumber,
            int pageSize,
            out int totalCount)
        {
            var inventories = _inventoryRepository.GetAllByPartnerId(partnerId).AsQueryable();

            // Lọc theo từ khóa
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                inventories = inventories.Where(i =>
                    (i.Material.MaterialName ?? "").Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    (i.Material.MaterialCode ?? "").Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    (i.Warehouse.WarehouseName ?? "").Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                );
            }

            totalCount = inventories.Count();

            if (pageNumber > 0 && pageSize > 0)
                inventories = inventories.Skip((pageNumber - 1) * pageSize).Take(pageSize);

            var result = inventories
                .ToList()
                .Select(i => new InventoryInfoDto
                {
                    MaterialId = i.MaterialId,
                    MaterialCode = i.Material.MaterialCode,
                    MaterialName = i.Material.MaterialName,
                    CategoryName = i.Material.Category?.CategoryName ?? "N/A",
                    Unit = i.Material.Unit,
                    WarehouseName = i.Warehouse.WarehouseName,
                    Quantity = i.Quantity ?? 0,
                    BatchNumber = i.BatchNumber,
                    ExpiryDate = i.ExpiryDate
                })
                .ToList();

            return result;
        }

    }
}
