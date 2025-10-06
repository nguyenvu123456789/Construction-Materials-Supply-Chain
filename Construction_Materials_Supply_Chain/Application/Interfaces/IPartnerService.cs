using Domain.Models;

namespace Application.Interfaces
{
    public interface IPartnerService
    {
        List<PartnerType> GetPartnerTypesWithPartners();
        List<Partner> GetPartnersByType(int partnerTypeId);
        List<Partner> GetPartnersFiltered(string? searchTerm, List<string>? partnerTypeNames, int pageNumber, int pageSize, out int totalCount);
    }
}
