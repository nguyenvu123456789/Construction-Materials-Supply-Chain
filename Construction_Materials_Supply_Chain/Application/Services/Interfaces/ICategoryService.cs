using Domain.Models;

namespace Application.Interfaces
{
    public interface ICategoryService
    {
        List<Category> GetAll();
        Category? GetById(int id);
        void Create(Category category);
        void Update(Category category);
        void Delete(int id);
        List<Category> GetCategoriesFiltered(string? searchTerm, int pageNumber, int pageSize, out int totalCount);
    }
}
