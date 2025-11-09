using Domain.Interface.Base;
using Domain.Models;

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
        IEnumerable<string> GetRoleNamesByUserId(int userId);
        void AssignRoles(int userId, IEnumerable<int> roleIds);
        IQueryable<User> QueryWithRolesAndPartner();
        User? GetByIdWithPartner(int id);
        int GetOrCreateSystemUserId(int partnerId);
        IEnumerable<string> GetEmailsByUserIds(IEnumerable<int> userIds);

    }
}
