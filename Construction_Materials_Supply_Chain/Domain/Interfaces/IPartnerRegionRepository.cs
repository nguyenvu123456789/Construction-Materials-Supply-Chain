using Domain.Interface.Base;
using Domain.Models;

namespace Domain.Interfaces
{
    public interface IPartnerRegionRepository : IGenericRepository<PartnerRegion>
    {
        List<Partner> GetPartnersWithRegionsByIds(List<int> partnerIds);
    }
}
