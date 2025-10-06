using Domain.Models;

namespace Application.Interfaces
{
    public interface IRoleService
    {
        List<Role> GetAll();
    }
}