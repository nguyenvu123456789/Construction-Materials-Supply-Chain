using Domain.Interface.Base;
using Domain.Models;

namespace Domain.Interfaces
{
    public interface IPriceMaterialPartnerRepository : IGenericRepository<PriceMaterialPartner>
    {
        IQueryable<PriceMaterialPartner> QueryAll();
        Task<PriceMaterialPartner?> GetByIdAsync(int id);
    }
}
