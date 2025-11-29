using Domain.Interface.Base;
using Domain.Models;
using System.Linq;
using System.Collections.Generic;

namespace Domain.Interface
{
    public interface IPartnerRelationRepository : IGenericRepository<PartnerRelation>
    {
        IQueryable<PartnerRelation> QueryWithRelations();
        List<PartnerRelation> GetRelationsByBuyer(int buyerPartnerId);
        PartnerRelation? GetRelation(int buyerPartnerId, int sellerPartnerId);
        Task<PartnerRelation?> GetRelationAsync(int buyerPartnerId, int sellerPartnerId);
    }
}
