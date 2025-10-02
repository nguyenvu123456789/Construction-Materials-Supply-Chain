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

        public List<Partner> GetPartnersPaged(string? keyword, int pageNumber, int pageSize)
            => _dao.GetPartnersPaged(keyword, pageNumber, pageSize);

        public int GetTotalPartnersCount(string? keyword)
            => _dao.GetTotalPartnersCount(keyword);
    }
}
