using BusinessObjects;
using DataAccess;
using Repositories.Interface;

namespace Repositories.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly RoleDAO _dao;

        public RoleRepository(RoleDAO dao)
        {
            _dao = dao;
        }

        public List<Role> GetRoles() => _dao.GetRoles();
    }
}