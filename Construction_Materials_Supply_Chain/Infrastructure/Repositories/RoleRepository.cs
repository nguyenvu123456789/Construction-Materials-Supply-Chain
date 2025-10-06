using Infrastructure.Persistence;
using Domain.Interface;
using Domain.Models;
using Infrastructure.Repositories.Base;

namespace Infrastructure.Implementations
{
    public class RoleRepository : GenericRepository<Role>, IRoleRepository
    {
        public RoleRepository(ScmVlxdContext context) : base(context) { }

        public List<Role> GetRoles() => _dbSet.ToList();
    }
}
