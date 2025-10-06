using Domain.Models;
using Domain.Interface.Base;

namespace Domain.Interface
{
    public interface IRoleRepository : IGenericRepository<Role>
    {
        List<Role> GetRoles();
    }
}
