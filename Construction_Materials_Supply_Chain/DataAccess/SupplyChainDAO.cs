using BusinessObjects;
using Microsoft.EntityFrameworkCore;

namespace DataAccess
{
    public class SupplyChainDAO : BaseDAO
    {
        public SupplyChainDAO(ScmVlxdContext context) : base(context) { }

        public List<Partner> GetPartners() =>
            Context.Partners.Include(p => p.PartnerType).ToList();

        public List<PartnerType> GetPartnerTypes() =>
            Context.PartnerTypes.Include(pt => pt.Partners).ToList();

        public List<Partner> GetPartnersPaged(string? keyword, int pageNumber, int pageSize)
        {
            var query = Context.Partners.Include(p => p.PartnerType).AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(p => p.PartnerName.Contains(keyword));
            }

            return query.OrderBy(p => p.PartnerId)
                        .Skip((pageNumber - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();
        }

        public int GetTotalPartnersCount(string? keyword)
        {
            var query = Context.Partners.AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(p => p.PartnerName.Contains(keyword));
            }

            return query.Count();
        }
    }
}
