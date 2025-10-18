using Domain.Interface.Base;
using Domain.Models;

namespace Domain.Interface
{
    public interface IAddressRepository : IGenericRepository<Address>
    {
        Address? GetByName(string name);
        List<Address> Search(string? q, string? city, int? top);
    }
}
