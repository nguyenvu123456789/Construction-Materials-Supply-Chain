using System.Collections.Generic;
using BusinessObjects;

namespace Services.Interfaces
{
    public interface IRoleService
    {
        List<Role> GetAll();
    }
}