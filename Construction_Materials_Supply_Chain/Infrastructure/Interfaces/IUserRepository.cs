using Domain;
using Infrastructure.Base;

namespace Infrastructure.Interface
{
    public interface IUserRepository : IGenericRepository<User>
    {
        User GetByUsername(string username);
    }
}
