using Domain.Interface;
using Domain.Models;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Repositories
{
    public class PartnerRelationRepository : GenericRepository<PartnerRelation>, IPartnerRelationRepository
    {
        private readonly ScmVlxdContext _context;
        public async Task<PartnerRelation?> GetRelationAsync(int buyerPartnerId, int sellerPartnerId)
        {
            return await _context.PartnerRelations
                .Include(pr => pr.RelationType)
                .FirstOrDefaultAsync(pr => pr.BuyerPartnerId == buyerPartnerId && pr.SellerPartnerId == sellerPartnerId);
        }
        public PartnerRelationRepository(ScmVlxdContext context) : base(context)
        {
            _context = context;
        }

        public IQueryable<PartnerRelation> QueryWithRelations()
        {
            return _context.PartnerRelations
                .Include(pr => pr.RelationType)
                .Include(pr => pr.BuyerPartner)
                .Include(pr => pr.SellerPartner);
        }

        public List<PartnerRelation> GetRelationsByBuyer(int buyerPartnerId)
        {
            return QueryWithRelations()
                .Where(pr => pr.BuyerPartnerId == buyerPartnerId)
                .ToList();
        }

        public PartnerRelation? GetRelation(int buyerPartnerId, int sellerPartnerId)
        {
            return QueryWithRelations()
                .FirstOrDefault(pr => pr.BuyerPartnerId == buyerPartnerId && pr.SellerPartnerId == sellerPartnerId);
        }
    }
}
