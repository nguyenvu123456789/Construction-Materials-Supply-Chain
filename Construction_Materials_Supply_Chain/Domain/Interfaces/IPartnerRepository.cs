using Domain.Interface.Base;
using Domain.Models;
using System.Linq;

namespace Domain.Interface
{
    public interface IPartnerRepository : IGenericRepository<Partner>
    {
        IQueryable<Partner> QueryWithType();
    }
}