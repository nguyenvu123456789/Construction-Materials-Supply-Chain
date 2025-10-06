using BusinessObjects;

namespace Repositories.Interface
{
    public interface ISupplyChainRepository
    {
        List<Partner> GetPartners();
        List<PartnerType> GetPartnerTypes();

        List<Partner> GetPartnersPaged(string? searchTerm, string? partnerType, int pageNumber, int pageSize);
        int GetTotalPartnersCount(string? searchTerm, string? partnerType);
    }
}