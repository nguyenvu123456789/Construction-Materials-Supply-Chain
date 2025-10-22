using Application.Interfaces;
using Domain.Interface;
using Domain.Models;

namespace Application.Services.Implements
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categories;
        private readonly IMaterialRepository _materials;

        public CategoryService(ICategoryRepository categories, IMaterialRepository materials)
        {
            _categories = categories;
            _materials = materials;
        }

        public List<Category> GetAll()
        {
            return _categories
                .GetAll()
                .Where(c => c.Status != "Deleted")
                .ToList();
        }

        public Category? GetById(int id)
        {
            var category = _categories.GetById(id);
            return category != null && category.Status != "Deleted" ? category : null;
        }

        public void Create(Category category)
        {
            if (_categories.ExistsByName(category.CategoryName))
                throw new Exception("Category name already exists.");

            category.CreatedAt = DateTime.UtcNow;
            category.Status = "Active";
            _categories.Add(category);
        }

        public void Update(Category category)
        {
            var existing = _categories.GetById(category.CategoryId);
            if (existing == null || existing.Status == "Deleted")
                throw new Exception("Category not found or deleted.");

            existing.CategoryName = category.CategoryName;
            existing.Description = category.Description;
            existing.Status = category.Status; // có thể chuyển giữa Active/Inactive

            _categories.Update(existing);
        }

        public void Delete(int id)
        {
            var cat = _categories.GetById(id);
            if (cat == null || cat.Status == "Deleted")
                throw new Exception("Category not found or already deleted.");

            // Soft delete category
            cat.Status = "Deleted";
            _categories.Update(cat);

            // Soft delete all related materials
            var materials = _materials.GetByCategory(id);
            foreach (var material in materials)
            {
                if (material.Status != "Deleted")
                {
                    material.Status = "Deleted";
                    _materials.Update(material);
                }
            }
        }

        public List<Category> GetCategoriesFiltered(string? searchTerm, int pageNumber, int pageSize, out int totalCount)
        {
            var query = _categories
                .GetAll()
                .Where(c => c.Status != "Deleted")
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
                query = query.Where(c => (c.CategoryName ?? "").Contains(searchTerm));

            totalCount = query.Count();

            if (pageNumber > 0 && pageSize > 0)
                query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);

            return query.ToList();
        }

        public List<Category> GetByStatus(string status)
        {
            return _categories
                .GetAll()
                .Where(c => c.Status.Equals(status, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
    }
}
