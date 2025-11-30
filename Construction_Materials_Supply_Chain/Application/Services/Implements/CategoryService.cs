using Application.Constants.Enums;
using Application.Constants.Messages;
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
            return _categories.GetAll()
                .Where(c => c.Status != StatusEnum.Deleted.ToStatusString())
                .ToList();
        }

        public Category? GetById(int id)
        {
            var category = _categories.GetById(id);
            if (category == null || category.Status == StatusEnum.Deleted.ToStatusString())
                return null;

            return category;
        }

        public void Create(Category category)
        {
            if (_categories.ExistsByName(category.CategoryName))
                throw new Exception(CategoryMessages.MSG_CATEGORY_NAME_EXISTS);

            category.Status = "Active";
            category.CreatedAt = DateTime.Now;

            _categories.Add(category);
        }

        public void Update(Category category)
        {
            var existing = _categories.GetById(category.CategoryId);
            if (existing == null || existing.Status == StatusEnum.Deleted.ToStatusString())
                throw new Exception(CategoryMessages.MSG_CATEGORY_NOT_FOUND);

            existing.CategoryName = category.CategoryName;
            existing.Description = category.Description;
            existing.Status = category.Status;

            _categories.Update(existing);
        }

        public void Delete(int id)
        {
            var category = _categories.GetById(id);
            if (category == null || category.Status == StatusEnum.Deleted.ToStatusString())
                throw new Exception(CategoryMessages.MSG_CATEGORY_NOT_FOUND);

            category.Status = StatusEnum.Deleted.ToStatusString()   ;
            _categories.Update(category);

            // Soft delete all related materials
            var materials = _materials.GetByCategory(id);
            foreach (var material in materials.Where(m => m.Status != StatusEnum.Deleted.ToStatusString()))
            {
                material.Status = StatusEnum.Deleted.ToStatusString();
                _materials.Update(material);
            }
        }

        public List<Category> GetCategoriesFiltered(string? searchTerm, int pageNumber, int pageSize, out int totalCount)
        {
            var query = _categories.GetAll()
                .Where(c => c.Status != StatusEnum.Deleted.ToStatusString())
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
            return _categories.GetAll()
                .Where(c => c.Status.Equals(status, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
    }
}
