using Application.DTOs;

namespace Application.Interfaces
{
    public interface IInventoryService
    {
        List<InventoryInfoDto> GetInventoryByPartnerFiltered(
            int partnerId,
            string? searchTerm,
            int pageNumber,
            int pageSize,
            out int totalCount
        );
    }
}
