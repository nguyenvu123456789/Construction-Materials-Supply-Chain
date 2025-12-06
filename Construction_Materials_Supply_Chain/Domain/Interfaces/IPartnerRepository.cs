using Domain.Interface.Base;
using Domain.Models;

namespace Domain.Interface
{
    public interface IPartnerRepository : IGenericRepository<Partner>
    {
        IQueryable<Partner> QueryWithType();
        IQueryable<Partner> QueryWithTypeIncludeDeleted();
        List<Partner> GetAllNotDeleted();
        Partner? GetByIdNotDeleted(int id);
        void SoftDelete(Partner entity);
        Partner? GetPartnerWithRegions(int partnerId);
        PartnerRelation? GetRelation(int buyerPartnerId, int sellerPartnerId);
        PriceMaterialPartner? GetPriceMaterial(int partnerId, int materialId);

    }
}
