using Domain.Interface.Base;
using Domain.Models;

namespace Domain.Interfaces
{
    public interface IPriceMaterialPartnerRepository : IGenericRepository<PriceMaterialPartner>
    {
        IQueryable<MaterialPartner> MaterialPartners();
        IQueryable<PriceMaterialPartner> Prices();
        void UpsertPrice(int partnerId, int materialId, decimal buyPrice, decimal sellPrice, string? status = null);
    }
}
