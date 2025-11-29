using Domain.Interfaces;
using Domain.Models;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class PriceMaterialPartnerRepository : GenericRepository<PriceMaterialPartner>, IPriceMaterialPartnerRepository
    {
        private readonly ScmVlxdContext _context;

        public PriceMaterialPartnerRepository(ScmVlxdContext context) : base(context)
        {
            _context = context;
        }

        public IQueryable<PriceMaterialPartner> QueryAll()
        {
            return _context.PriceMaterialPartners
                .Include(x => x.Partner)
                .Include(x => x.Material)
                .AsNoTracking()
                .AsQueryable();
        }

        public async Task<PriceMaterialPartner?> GetByIdAsync(int id)
        {
            return await _context.PriceMaterialPartners
                .Include(x => x.Partner)
                .Include(x => x.Material)
                .FirstOrDefaultAsync(x => x.PriceMaterialPartnerId == id);
        }
    }
}
