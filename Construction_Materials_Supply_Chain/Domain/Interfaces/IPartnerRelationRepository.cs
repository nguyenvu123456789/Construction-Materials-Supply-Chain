using Domain.Interface.Base;
using Domain.Models;

namespace Domain.Interface
{
    public interface IPartnerRelationRepository : IGenericRepository<PartnerRelation>
    {
        IQueryable<PartnerRelation> QueryWithRelations();
        List<PartnerRelation> GetRelationsByBuyer(int buyerPartnerId);
        List<PartnerRelation> GetRelationsBySeller(int sellerPartnerId);
        PartnerRelation? GetRelation(int buyerPartnerId, int sellerPartnerId);
        Task<PartnerRelation?> GetRelationAsync(int buyerPartnerId, int sellerPartnerId);
    }
}
