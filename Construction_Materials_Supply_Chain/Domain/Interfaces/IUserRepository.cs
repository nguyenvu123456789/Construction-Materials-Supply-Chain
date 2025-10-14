using Domain.Interface.Base;
using Domain.Models;
using System.Linq;

namespace Domain.Interface
{
    public interface IUserRepository : IGenericRepository<User>
    {
        List<User> GetAllNotDeleted();
        User? GetByIdNotDeleted(int id);
        User? GetByUsername(string username);
        bool ExistsByUsername(string username);
        IQueryable<User> QueryWithRoles();
        IQueryable<User> QueryWithRolesIncludeDeleted();
        void SoftDelete(User entity);
    }
}
