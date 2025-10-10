using Domain.Interface.Base;
using Domain.Models;

namespace Domain.Interface
{
    public interface IRoleRepository : IGenericRepository<Role>
    {
        List<Role> GetRoles();
    }
}
