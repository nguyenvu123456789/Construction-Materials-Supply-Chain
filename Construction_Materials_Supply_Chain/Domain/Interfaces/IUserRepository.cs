using Domain.Interface.Base;
using Domain.Models;

namespace Domain.Interface
{
    public interface IUserRepository : IGenericRepository<User>
    {
        User GetByUsername(string username);
        bool ExistsByUsername(string username);
    }
}
