using BusinessObjects;

namespace Repositories.Interface
{
    public interface ISupplyChainRepository
    {
        List<Partner> GetPartners();
        List<PartnerType> GetPartnerTypes();

        List<Partner> GetPartnersPaged(string? keyword, int pageNumber, int pageSize);
        int GetTotalPartnersCount(string? keyword);
    }
}
