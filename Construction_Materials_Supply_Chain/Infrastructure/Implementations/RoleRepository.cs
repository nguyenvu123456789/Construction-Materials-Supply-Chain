using Domain;
using Infrastructure.Persistence;
using Infrastructure.Base;
using Infrastructure.Interface;

namespace Infrastructure.Implementations
{
    public class RoleRepository : GenericRepository<Role>, IRoleRepository
    {
        public RoleRepository(ScmVlxdContext context) : base(context) { }

        public List<Role> GetRoles() => _dbSet.ToList();
    }
}
