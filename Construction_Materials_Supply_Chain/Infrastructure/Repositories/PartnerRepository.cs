using Domain.Interface;
using Domain.Models;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Implementations
{
    public class PartnerRepository : GenericRepository<Partner>, IPartnerRepository
    {
        public PartnerRepository(ScmVlxdContext context) : base(context)
        {
        }

        public IQueryable<Partner> QueryWithType()
            => _dbSet.AsNoTracking().Include(p => p.PartnerType);
    }
}
