using System.Collections.Generic;
using BusinessObjects;
using Repositories.Base;

namespace Repositories.Interface
{
    public interface IRoleRepository : IGenericRepository<Role>
    {
        List<Role> GetRoles();
    }
}
