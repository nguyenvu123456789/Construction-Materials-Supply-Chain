using Domain.Interface;
using Domain.Models;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Base;

namespace Infrastructure.Implementations
{
    public class RoleRepository : GenericRepository<Role>, IRoleRepository
    {
        public RoleRepository(ScmVlxdContext context) : base(context) { }

        public IQueryable<Role> Query()
            => _dbSet.AsQueryable();
    }
}
