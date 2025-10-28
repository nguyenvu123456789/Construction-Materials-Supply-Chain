using Application.DTOs;

namespace Application.Interfaces
{
    public interface IInventoryService
    {
        List<InventoryInfoDto> GetInventoryByPartner(int partnerId);
    }
}
