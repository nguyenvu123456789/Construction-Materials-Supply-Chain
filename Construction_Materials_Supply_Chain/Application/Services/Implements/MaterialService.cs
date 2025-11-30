using Application.Constants.Enums;
using Application.Constants.Messages;
using Application.DTOs;
using Application.DTOs.Material;
using Application.Interfaces;
using Domain.Interface;
using Domain.Models;

namespace Application.Services.Implements
{
    public class MaterialService : IMaterialService
    {
        private readonly IMaterialRepository _materials;

        public MaterialService(IMaterialRepository materials)
        {
            _materials = materials;
        }

        public List<Material> GetAll() =>
            _materials.GetAllWithInclude()
                      .Where(m => m.Status != StatusEnum.Deleted.ToStatusString())
                      .ToList();

        public MaterialDetailResponse? GetById(int id) => GetById(id, null);

        public MaterialDetailResponse? GetById(int id, int? buyerPartnerId)
        {
            var material = _materials.GetDetailById(id);
            if (material == null) return null;

            var materialDto = new MaterialDto
            {
                MaterialId = material.MaterialId,
                MaterialCode = material.MaterialCode,
                MaterialName = material.MaterialName,
                CategoryId = material.CategoryId,
                CategoryName = material.Category?.CategoryName,
                Unit = material.Unit,
                CreatedAt = material.CreatedAt
            };

            var materialPartners = material.MaterialPartners.AsQueryable();
            if (buyerPartnerId.HasValue)
                materialPartners = materialPartners.Where(mp => mp.BuyerId == buyerPartnerId.Value).AsQueryable();

            var partners = materialPartners.Select(mp => new PartnerDto
            {
                PartnerId = mp.Partner.PartnerId,
                PartnerCode = mp.Partner.PartnerCode,
                PartnerName = mp.Partner.PartnerName,
                ContactEmail = mp.Partner.ContactEmail,
                ContactPhone = mp.Partner.ContactPhone,
                PartnerTypeId = mp.Partner.PartnerTypeId,
                PartnerTypeName = mp.Partner.PartnerType.TypeName,
                Status = mp.Partner.Status ?? StatusEnum.Active.ToStatusString(),
            }).ToList();

            return new MaterialDetailResponse
            {
                Material = materialDto,
                Partners = partners
            };
        }

        public void CreateMaterial(CreateMaterialRequest request)
        {
            if (_materials.ExistsByName(request.MaterialName))
                throw new Exception(MaterialMessages.MSG_MATERIAL_NAME_EXISTS);

            var material = new Material
            {
                MaterialCode = request.MaterialCode,
                MaterialName = request.MaterialName,
                CategoryId = request.CategoryId,
                Unit = request.Unit,
                CreatedAt = DateTime.Now,
                Status = StatusEnum.Active.ToStatusString()
            };

            _materials.Add(material);

            var inventory = new Inventory
            {
                MaterialId = material.MaterialId,
                WarehouseId = request.WarehouseId,
                Quantity = 0,
                CreatedAt = DateTime.Now
            };

            _materials.AddInventory(inventory);
        }

        public void UpdateMaterial(int id, UpdateMaterialRequest request)
        {
            var existing = _materials.GetById(id);
            if (existing == null || existing.Status == StatusEnum.Deleted.ToStatusString())
                throw new KeyNotFoundException(MaterialMessages.MSG_MATERIAL_NOT_FOUND);

            existing.MaterialName = request.MaterialName;
            existing.MaterialCode = request.MaterialCode;
            existing.Unit = request.Unit;
            existing.CategoryId = request.CategoryId;
            existing.Status = request.Status;

            _materials.Update(existing);
        }

        public void Delete(int id)
        {
            var mat = _materials.GetById(id);
            if (mat == null || mat.Status == StatusEnum.Deleted.ToStatusString())
                throw new KeyNotFoundException(MaterialMessages.MSG_MATERIAL_NOT_FOUND);

            mat.Status = StatusEnum.Deleted.ToStatusString();
            _materials.Update(mat);
        }

        public List<Material> GetMaterialsFiltered(string? searchTerm, int pageNumber, int pageSize, out int totalCount)
        {
            var query = _materials.GetAllWithInventory()
                                  .Where(m => m.Status != StatusEnum.Deleted.ToStatusString())
                                  .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
                query = query.Where(m => (m.MaterialName ?? "").Contains(searchTerm)
                                       || (m.MaterialCode ?? "").Contains(searchTerm));

            totalCount = query.Count();

            if (pageNumber > 0 && pageSize > 0)
                query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);

            return query.ToList();
        }

        public List<Material> GetByCategoryOrFail(int categoryId)
        {
            var materials = _materials.GetByCategory(categoryId)
                .Where(m => m.Status != StatusEnum.Deleted.ToStatusString())
                .ToList();

            if (!materials.Any())
                throw new KeyNotFoundException(MaterialMessages.MSG_NO_MATERIALS_IN_CATEGORY);

            return materials;
        }

        public List<Material> GetByWarehouseOrFail(int warehouseId, string? searchTerm)
        {
            var materials = _materials.GetByWarehouse(warehouseId)
                .Where(m => m.Status != StatusEnum.Deleted.ToStatusString())
                .ToList();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                materials = materials
                    .Where(m => (m.MaterialName ?? "").Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                             || (m.MaterialCode ?? "").Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            if (!materials.Any())
                throw new KeyNotFoundException(MaterialMessages.MSG_NO_MATERIALS_IN_WAREHOUSE);

            return materials;
        }

        public List<Material> GetByStatus(string status)
        {
            return _materials.GetAllWithInclude()
                             .Where(m => m.Status.Equals(status, StringComparison.OrdinalIgnoreCase))
                             .ToList();
        }
    }
}
