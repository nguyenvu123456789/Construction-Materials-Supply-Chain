using Application.DTOs.Material;
using Domain.Models;

namespace Application.Interfaces
{
    public interface IMaterialService
    {
        List<Material> GetAll();
        MaterialDetailResponse? GetById(int id);
        MaterialDetailResponse? GetById(int id, int? buyerPartnerId);
        void CreateMaterial(CreateMaterialRequest request);
        void UpdateMaterial(int id, UpdateMaterialRequest request);
        void Delete(int id);
        List<Material> GetMaterialsFiltered(string? searchTerm, int pageNumber, int pageSize, out int totalCount);
        List<Material> GetByCategoryOrFail(int categoryId);
        List<Material> GetByWarehouseOrFail(int warehouseId, string? searchTerm);


    }
}
