using Domain.Interface.Base;
using Domain.Models;

namespace Domain.Interface
{
    public interface IPartnerRepository : IGenericRepository<Partner>
    {
        IQueryable<Partner> QueryWithType();
    }
}