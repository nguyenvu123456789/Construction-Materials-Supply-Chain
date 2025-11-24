using Application.DTOs;

namespace Application.Interfaces
{
    public interface IInventoryService
    {
        List<InventoryInfoDto> GetInventoryFiltered(
            int? partnerId,
            int? managerId,
            string? searchTerm,
            int pageNumber,
            int pageSize,
            out int totalCount
        );
    }
}
