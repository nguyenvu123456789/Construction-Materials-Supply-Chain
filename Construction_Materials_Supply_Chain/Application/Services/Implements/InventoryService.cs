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

        public List<InventoryInfoDto> GetInventoryByPartner(int partnerId)
        {
            var inventories = _inventoryRepository.GetAllByPartnerId(partnerId);

            return inventories.Select(i => new InventoryInfoDto
            {
                MaterialId = i.MaterialId,
                MaterialName = i.Material.MaterialName,
                CategoryName = i.Material.Category?.CategoryName ?? "N/A",
                Unit = i.Material.Unit,
                WarehouseName = i.Warehouse.WarehouseName,
                Quantity = i.Quantity ?? 0,
                BatchNumber = i.BatchNumber,
                ExpiryDate = i.ExpiryDate
            }).ToList();
        }
    }
}
