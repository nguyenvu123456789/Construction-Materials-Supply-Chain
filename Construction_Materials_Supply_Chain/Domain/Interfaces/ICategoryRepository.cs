using Domain.Interface.Base;
using Domain.Models;

namespace Domain.Interface
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        Category? GetByName(string name);
        bool ExistsByName(string name);
    }
}
