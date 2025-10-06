using Domain.Models;
using Domain.Interface.Base;

namespace Domain.Interface
{
    public interface IUserRepository : IGenericRepository<User>
    {
        User GetByUsername(string username);
        bool ExistsByUsername(string username);
    }
}
