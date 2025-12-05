using Domain.Interface;
using Domain.Models;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Implementations
{
    public class RelationTypeRepository : GenericRepository<RelationType>, IRelationTypeRepository
    {
        public RelationTypeRepository(ScmVlxdContext context) : base(context) { }

        public IQueryable<RelationType> Query()
            => _dbSet.AsNoTracking()
                     .Where(x => x.Status != "Deleted");

        public IQueryable<RelationType> QueryIncludeRelations()
            => _dbSet.AsNoTracking()
                     .Include(x => x.PartnerRelations)
                     .ThenInclude(pr => pr.BuyerPartner)
                     .Include(x => x.PartnerRelations)
                     .ThenInclude(pr => pr.SellerPartner);
    }
}
