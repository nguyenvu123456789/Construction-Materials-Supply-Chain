using System.Collections.Generic;
using BusinessObjects;

namespace Services.Interfaces
{
    public interface IPartnerService
    {
        List<PartnerType> GetPartnerTypesWithPartners();
        List<Partner> GetPartnersByType(int partnerTypeId);
        List<Partner> GetPartnersFiltered(string? searchTerm, string? partnerTypeName, int pageNumber, int pageSize, out int totalCount);
    }
}
