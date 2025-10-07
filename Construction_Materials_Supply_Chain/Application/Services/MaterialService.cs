using Application.Interfaces;
using Domain.Interface;
using Domain.Models;

namespace Services.Implementations
{
    public class MaterialService : IMaterialService
    {
        private readonly IMaterialRepository _materials;

        public MaterialService(IMaterialRepository materials)
        {
            _materials = materials;
        }

        public List<Material> GetAll() => _materials.GetAllWithInclude();

        public Material? GetById(int id) => _materials.GetByIdWithInclude(id);

        public void Create(Material material)
        {
            if (_materials.ExistsByName(material.MaterialName))
                throw new Exception("Material name already exists.");

            material.CreatedAt = DateTime.UtcNow;
            _materials.Add(material);
        }

        public void Update(Material material)
        {
            var existing = _materials.GetById(material.MaterialId);
            if (existing == null) throw new Exception("Material not found.");

            existing.MaterialName = material.MaterialName;
            existing.MaterialCode = material.MaterialCode;
            existing.Unit = material.Unit;
            existing.CategoryId = material.CategoryId;
            existing.PartnerId = material.PartnerId;

            _materials.Update(existing);
        }

        public void Delete(int id)
        {
            var mat = _materials.GetById(id);
            if (mat != null)
                _materials.Delete(mat);
        }

        public List<Material> GetMaterialsFiltered(string? searchTerm, int pageNumber, int pageSize, out int totalCount)
        {
            var query = _materials.GetAllWithInventory().AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
                query = query.Where(m => (m.MaterialName ?? "").Contains(searchTerm)
                                       || (m.MaterialCode ?? "").Contains(searchTerm));

            totalCount = query.Count();

            if (pageNumber > 0 && pageSize > 0)
                query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);

            return query.ToList();
        }

        public List<Material> GetByCategory(int categoryId)
        {
            return _materials.GetByCategory(categoryId);
        }
        public List<Material> GetByWarehouse(int warehouseId, string? searchTerm)
        {
            var materials = _materials.GetByWarehouse(warehouseId);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                materials = materials
                    .Where(m => (m.MaterialName ?? "").Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                             || (m.MaterialCode ?? "").Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            return materials;
        }

    }
}
