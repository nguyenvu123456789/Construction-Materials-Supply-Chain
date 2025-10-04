using System.Collections.Generic;
using System.Linq;
using BusinessObjects;
using DataAccess;
using Repositories.Base;
using Repositories.Interface;

namespace Repositories.Implementations
{
    public class RoleRepository : GenericRepository<Role>, IRoleRepository
    {
        public RoleRepository(ScmVlxdContext context) : base(context) { }

        public List<Role> GetRoles() => _dbSet.ToList();
    }
}
