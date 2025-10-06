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

        public List<Partner> GetPartnersPaged(string? searchTerm, string? partnerType, int pageNumber, int pageSize)
        {
            var query = Context.Partners
                              .Include(p => p.PartnerType)
                              .AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(p => p.PartnerName.Contains(searchTerm));
            }

            if (!string.IsNullOrEmpty(partnerType))
            {
                if (int.TryParse(partnerType, out int partnerTypeId))
                {
                    query = query.Where(p => p.PartnerTypeId == partnerTypeId);
                }
                else
                {
                    query = query.Where(p => p.PartnerType.TypeName.Contains(partnerType));
                }
            }

            return query.OrderBy(p => p.PartnerId)
                        .Skip((pageNumber - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();
        }

        public int GetTotalPartnersCount(string? searchTerm, string? partnerType)
        {
            var query = Context.Partners
                              .Include(p => p.PartnerType)
                              .AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(p => p.PartnerName.Contains(searchTerm));
            }

            if (!string.IsNullOrEmpty(partnerType))
            {
                if (int.TryParse(partnerType, out int partnerTypeId))
                {
                    query = query.Where(p => p.PartnerTypeId == partnerTypeId);
                }
                else
                {
                    query = query.Where(p => p.PartnerType.TypeName.Contains(partnerType));
                }
            }

            return query.Count();
        }
    }
}