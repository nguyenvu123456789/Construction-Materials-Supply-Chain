using Application.DTOs.Material;
using Domain.Models;

namespace Application.Interfaces
{
    public interface IMaterialService
    {
        List<Material> GetAll();
        MaterialDetailResponse? GetById(int id);
        MaterialDetailResponse? GetById(int id, int? buyerPartnerId);
        void CreateWithInventory(Material material, int warehouseId);
        void Update(Material material);
        void Delete(int id);
        List<Material> GetMaterialsFiltered(string? searchTerm, int pageNumber, int pageSize, out int totalCount);
        List<Material> GetByCategory(int categoryId);
        List<Material> GetByWarehouse(int warehouseId, string? searchTerm);


    }
}
