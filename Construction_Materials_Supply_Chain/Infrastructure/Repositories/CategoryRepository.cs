using Domain.Interface;
using Domain.Models;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Base;

namespace Infrastructure.Implementations
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(ScmVlxdContext context) : base(context) { }

        public Category? GetByName(string name)
        {
            return _dbSet.FirstOrDefault(c => c.CategoryName == name);
        }

        public bool ExistsByName(string name)
        {
            return _dbSet.Any(c => c.CategoryName == name);
        }
    }
}
