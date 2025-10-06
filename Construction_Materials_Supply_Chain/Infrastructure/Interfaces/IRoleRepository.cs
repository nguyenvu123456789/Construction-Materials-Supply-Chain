using Domain;
using Infrastructure.Base;

namespace Infrastructure.Interface
{
    public interface IRoleRepository : IGenericRepository<Role>
    {
        List<Role> GetRoles();
    }
}
