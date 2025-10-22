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

            public List<Material> GetAll()
            {
                return _materials
                    .GetAllWithInclude()
                    .Where(m => m.Status != "Deleted")
                    .ToList();
            }

            public Material? GetById(int id)
            {
                var material = _materials.GetByIdWithInclude(id);
                return material != null && material.Status != "Deleted" ? material : null;
            }

            public void Create(Material material)
            {
                if (_materials.ExistsByName(material.MaterialName))
                    throw new Exception("Material name already exists.");

                material.CreatedAt = DateTime.UtcNow;
                material.Status = "Active";
                _materials.Add(material);
            }

            public void Update(Material material)
            {
                var existing = _materials.GetById(material.MaterialId);
                if (existing == null || existing.Status == "Deleted")
                    throw new Exception("Material not found or deleted.");

                existing.MaterialName = material.MaterialName;
                existing.MaterialCode = material.MaterialCode;
                existing.Unit = material.Unit;
                existing.CategoryId = material.CategoryId;
                existing.PartnerId = material.PartnerId;
                existing.Status = material.Status; // có thể chuyển Active <-> Inactive

                _materials.Update(existing);
            }

            public void Delete(int id)
            {
                var mat = _materials.GetById(id);
                if (mat != null && mat.Status != "Deleted")
                {
                    mat.Status = "Deleted"; // soft delete
                    _materials.Update(mat);
                }
            }

            public List<Material> GetMaterialsFiltered(string? searchTerm, int pageNumber, int pageSize, out int totalCount)
            {
                var query = _materials.GetAllWithInventory()
                    .Where(m => m.Status != "Deleted")
                    .AsQueryable();

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
                return _materials.GetByCategory(categoryId)
                    .Where(m => m.Status != "Deleted")
                    .ToList();
            }

            public List<Material> GetByWarehouse(int warehouseId, string? searchTerm)
            {
                var materials = _materials.GetByWarehouse(warehouseId)
                    .Where(m => m.Status != "Deleted")
                    .ToList();

                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    materials = materials
                        .Where(m => (m.MaterialName ?? "").Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                                 || (m.MaterialCode ?? "").Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }

                return materials;
            }

            public List<Material> GetByStatus(string status)
            {
                return _materials
                    .GetAllWithInclude()
                    .Where(m => m.Status.Equals(status, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
        }
    }
