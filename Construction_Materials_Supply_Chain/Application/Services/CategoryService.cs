using Application.Interfaces;
using Domain.Interface;
using Domain.Models;

namespace Services.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categories;

        public CategoryService(ICategoryRepository categories)
        {
            _categories = categories;
        }

        public List<Category> GetAll() => _categories.GetAll();

        public Category? GetById(int id) => _categories.GetById(id);

        public void Create(Category category)
        {
            if (_categories.ExistsByName(category.CategoryName))
                throw new Exception("Category name already exists.");
            category.CreatedAt = DateTime.UtcNow;
            _categories.Add(category);
        }

        public void Update(Category category)
        {
            var existing = _categories.GetById(category.CategoryId);
            if (existing == null) throw new Exception("Category not found.");

            existing.CategoryName = category.CategoryName;
            existing.Description = category.Description;
            _categories.Update(existing);
        }

        public void Delete(int id)
        {
            var cat = _categories.GetById(id);
            if (cat != null)
                _categories.Delete(cat);
        }

        public List<Category> GetCategoriesFiltered(string? searchTerm, int pageNumber, int pageSize, out int totalCount)
        {
            var query = _categories.GetAll().AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
                query = query.Where(c => (c.CategoryName ?? "").Contains(searchTerm));

            totalCount = query.Count();

            if (pageNumber > 0 && pageSize > 0)
                query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);

            return query.ToList();
        }
    }
}
