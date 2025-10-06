using BusinessObjects;
using DataAccess;
using Repositories.Interface;

namespace Repositories.Repositories
{
    public class SupplyChainRepository : ISupplyChainRepository
    {
        private readonly SupplyChainDAO _dao;

        public SupplyChainRepository(SupplyChainDAO dao)
        {
            _dao = dao;
        }

        public List<Partner> GetPartners() => _dao.GetPartners();
        public List<PartnerType> GetPartnerTypes() => _dao.GetPartnerTypes();

        public List<Partner> GetPartnersPaged(string? searchTerm, string? partnerType, int pageNumber, int pageSize)
            => _dao.GetPartnersPaged(searchTerm, partnerType, pageNumber, pageSize);

        public int GetTotalPartnersCount(string? searchTerm, string? partnerType)
            => _dao.GetTotalPartnersCount(searchTerm, partnerType);
    }
}