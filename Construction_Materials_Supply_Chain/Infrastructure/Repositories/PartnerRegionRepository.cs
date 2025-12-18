using Domain.Interfaces;
using Domain.Models;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class PartnerRegionRepository : GenericRepository<PartnerRegion>, IPartnerRegionRepository
    {
        public PartnerRegionRepository(ScmVlxdContext context): base(context)
        {
        }

        public List<Partner> GetPartnersWithRegionsByIds(List<int> partnerIds)
        {
            return _context.Partners
                .Include(p => p.PartnerRegions)
                    .ThenInclude(pr => pr.Region)
                .Where(p => partnerIds.Contains(p.PartnerId))
                .ToList();
        }

    }
}
