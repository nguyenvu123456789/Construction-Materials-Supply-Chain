using Domain.Interface;
using Domain.Models;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Implementations
{
    public class PartnerRepository : GenericRepository<Partner>, IPartnerRepository
    {
        public PartnerRepository(ScmVlxdContext context) : base(context) { }

        public IQueryable<Partner> QueryWithType()
            => _dbSet.AsNoTracking()
                     .Include(p => p.PartnerType)
                     .Include(p => p.PartnerRegions)
                         .ThenInclude(pr => pr.Region)
                     .Where(p => p.Status != "Deleted");

        public IQueryable<Partner> QueryWithTypeIncludeDeleted()
            => _dbSet
                     .Include(p => p.PartnerType)
                     .Include(p => p.PartnerRegions)
                         .ThenInclude(pr => pr.Region);

        public List<Partner> GetAllNotDeleted()
            => _dbSet.AsNoTracking()
                     .Where(p => p.Status != "Deleted")
                     .ToList();

        public Partner? GetByIdNotDeleted(int id)
            => _dbSet.FirstOrDefault(p => p.PartnerId == id && p.Status != "Deleted");

        public void SoftDelete(Partner entity)
        {
            entity.Status = "Deleted";
            _dbSet.Update(entity);
            _context.SaveChanges();
        }
    }
}
