using Domain.Interface.Base;
using Domain.Models;

namespace Domain.Interface
{
    public interface ITransportRepository : IGenericRepository<Transport>
    {
        Transport? GetById(int id);
    }
}
