using BusinessObjects;
using Repositories.Base;

namespace Repositories.Interface
{
    public interface IUserRepository : IGenericRepository<User>
    {
        User GetByUsername(string username);
    }
}
